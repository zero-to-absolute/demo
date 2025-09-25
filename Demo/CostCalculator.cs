using System;
using System.Data.SqlClient;
using System.Configuration;

namespace Demo
{
    public static class CostCalculator
    {
        /// <summary>
        /// Расчет себестоимости услуги
        /// Себестоимость = Стоимость материалов + Трудозатраты
        /// </summary>
        /// <param name="serviceId">Идентификатор услуги</param>
        /// <param name="connectionString">Строка подключения к БД</param>
        /// <returns>Себестоимость услуги или -1 в случае ошибки</returns>
        public static decimal CalculateServiceCost(int serviceId, string connectionString)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Получение базовой информации об услуге
                    string serviceQuery = @"
                        SELECT 
                            s.[Минимальная стоимость для партнера],
                            st.[Коэффициент сложности услуги],
                            s.[Основной материал]
                        FROM Services_import s
                        INNER JOIN Service_type_import st ON s.[Тип услуги] = st.[Тип услуги]
                        WHERE s.ServiceId = @ServiceId";

                    using (SqlCommand serviceCommand = new SqlCommand(serviceQuery, connection))
                    {
                        serviceCommand.Parameters.AddWithValue("@ServiceId", serviceId);
                        using (SqlDataReader reader = serviceCommand.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                return -1; // Услуга не найдена
                            }

                            decimal baseCost = Convert.ToDecimal(reader["Минимальная стоимость для партнера"]);
                            decimal complexityCoeff = Convert.ToDecimal(reader["Коэффициент сложности услуги"]);
                            string materialType = reader["Основной материал"]?.ToString();

                            reader.Close();

                            // Расчет стоимости материалов с учетом перерасхода
                            decimal materialCost = baseCost * 0.3m; // Предполагаем 30% от базовой стоимости на материалы

                            if (!string.IsNullOrEmpty(materialType))
                            {
                                string materialQuery = @"
                                    SELECT [Процент перерасхода материала]
                                    FROM Material_type_import 
                                    WHERE [Тип материала] = @MaterialType";

                                using (SqlCommand materialCommand = new SqlCommand(materialQuery, connection))
                                {
                                    materialCommand.Parameters.AddWithValue("@MaterialType", materialType);
                                    var overspendObj = materialCommand.ExecuteScalar();

                                    if (overspendObj != null)
                                    {
                                        decimal overspendPercent = Convert.ToDecimal(overspendObj);
                                        materialCost *= (1 + overspendPercent); // Учитываем перерасход
                                    }
                                }
                            }

                            // Расчет трудозатрат (предполагаем часовую ставку 500 руб/час)
                            decimal laborCost = baseCost * complexityCoeff * 0.4m; // 40% от базовой стоимости с учетом сложности

                            return materialCost + laborCost;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return -1; // Ошибка при расчете
            }
        }

        /// <summary>
        /// Расчет количества материала для услуги с учетом перерасхода
        /// </summary>
        /// <param name="serviceTypeId">Идентификатор типа услуги</param>
        /// <param name="materialTypeId">Идентификатор типа материала</param>
        /// <param name="serviceQuantity">Количество услуг</param>
        /// <param name="serviceParameters">Параметры услуги (вес, площадь и т.д.)</param>
        /// <param name="connectionString">Строка подключения к БД</param>
        /// <returns>Количество необходимого материала или -1 в случае ошибки</returns>
        public static int CalculateMaterialQuantity(int serviceTypeId, int materialTypeId,
            int serviceQuantity, double[] serviceParameters, string connectionString)
        {
            try
            {
                if (serviceQuantity <= 0 || serviceParameters == null || serviceParameters.Length == 0)
                    return -1;

                // Проверяем все параметры на положительность
                foreach (var param in serviceParameters)
                {
                    if (param <= 0)
                        return -1;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Получение коэффициента сложности услуги
                    string serviceTypeQuery = @"
                        SELECT [Коэффициент сложности услуги]
                        FROM Service_type_import 
                        WHERE ServiceTypeId = @ServiceTypeId";

                    decimal complexityCoeff = 0;
                    using (SqlCommand command = new SqlCommand(serviceTypeQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ServiceTypeId", serviceTypeId);
                        var result = command.ExecuteScalar();
                        if (result == null)
                            return -1;

                        complexityCoeff = Convert.ToDecimal(result);
                    }

                    // Получение процента перерасхода материала
                    string materialQuery = @"
                        SELECT [Процент перерасхода материала]
                        FROM Material_type_import 
                        WHERE MaterialTypeId = @MaterialTypeId";

                    decimal overspendPercent = 0;
                    using (SqlCommand command = new SqlCommand(materialQuery, connection))
                    {
                        command.Parameters.AddWithValue("@MaterialTypeId", materialTypeId);
                        var result = command.ExecuteScalar();
                        if (result == null)
                            return -1;

                        overspendPercent = Convert.ToDecimal(result);
                    }

                    // Расчет базового количества материала
                    double baseQuantity = 1.0; // Базовое количество на одну услугу

                    // Умножаем на все параметры услуги
                    foreach (var param in serviceParameters)
                    {
                        baseQuantity *= param;
                    }

                    // Умножаем на коэффициент типа услуги
                    baseQuantity *= (double)complexityCoeff;

                    // Умножаем на количество услуг
                    baseQuantity *= serviceQuantity;

                    // Учитываем перерасход материала
                    baseQuantity *= (1.0 + (double)overspendPercent);

                    // Округляем вверх до целого числа
                    return (int)Math.Ceiling(baseQuantity);
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
