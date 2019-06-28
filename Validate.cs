public static class Validate
    {
        public static bool ValidateMode<T>(this T t)
        {
            return ValidateMode(t, null);
        }

        public static bool ValidateMode<T>(this T t, object ValidateList)
        {
            try
            {
                if (t == null)
                {
                    return false;
                }
                Type type = t.GetType();
                if (type.Name.StartsWith("String"))
                {
                    return false;
                }
                PropertyInfo[] properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                if (properties.Length <= 0)
                {
                    return false;
                }

                var lstr = new List<string>();
                if (ValidateList != null)
                {
                    PropertyInfo[] properties1 = ValidateList.GetType().GetProperties();
                    foreach (var item in properties1)
                    {
                        lstr.Add(item.Name);
                    }
                }

                for (int i = 0; i < properties.Length; i++)
                {
                    var modelType = properties[i].PropertyType;
                    if (modelType.Name.StartsWith("String"))
                    {
                        string value = properties[i].GetValue(t, null)?.ToString();
                        if (value == null)
                        {
                            //continue;
                            if (lstr.Any(item => item == properties[i].Name))
                            {
                                //如果标记的值为空则不予通过
                                return false;
                            }
                        }
                        else
                        {
                            if (value != value.Trim())
                            {
                                properties[i].SetValue(t, value.Trim());
                            }
                        }
                    }
                    else if (modelType.Name.StartsWith("List"))
                    {
                        //TODO Continue recursion
                    }
                    else if (modelType.IsValueType)
                    {
                        if (modelType.Name.StartsWith("Int") || modelType.Name.StartsWith("Double"))
                        {

                        }
                        else if (modelType.Name.StartsWith("Boolean"))
                        {

                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        ValidateMode(properties[i].GetValue(t, null), null);
                    }
                }

                return true;
                //return t;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
        {
            MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
            return expressionBody.Member.Name;
        }


    }