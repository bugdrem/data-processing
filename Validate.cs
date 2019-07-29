public static class Validate
    {
        //Determine if the parameter is legal
        private static readonly List<string> _lstr = new List<string>();

        public static T ModelTrim<T>(this T t) where T : class
        {
            _lstr.Clear();
            return ValidateMode(t);
        }
        public static T ModelTrim<T>(this T t, object ValidateList) where T : class
        {
            _lstr.Clear();
            if (ValidateList != null)
            {
                PropertyInfo[] properties = ValidateList.GetType().GetProperties();
                foreach (var item in properties)
                {
                    _lstr.Add(item.Name);
                }
            }
            return ValidateMode(t);
        }

        private static T ValidateMode<T>(this T t)
        {
            try
            {
                if (t == null)
                {
                    return default;
                }
                Type type = t.GetType();

                if (type.Name == "String")
                {
                    var str = t.ToString().Trim();
                    if (string.IsNullOrEmpty(str))
                    {
                        return default;
                    }
                    return (T)(object)str;
                }
                else if (type.Name.Contains("List"))    //TODO Unrealized LinkedList String[]
                {
                    var gt = t.GetType();
                    if (gt.GenericTypeArguments.Count() == 1)
                    {
                        var gta = gt.GenericTypeArguments[0];
                        if (gta.Name == "String")
                        {
                            dynamic dd = t;
                            var index = 0;
                            foreach (var item in dd.ToArray())
                            {
                                if (item != null)
                                {
                                    if (item != item.Trim())
                                    {
                                        string newItem = item.Trim();
                                        if (string.IsNullOrEmpty(newItem))
                                        {
                                            dd[index] = null;
                                        }
                                        else
                                        {
                                            dd[index] = newItem;
                                        }
                                    }
                                    index++;
                                }
                            }
                        }
                        else if (gta.IsValueType)
                        {
                            // Int* Double Boolean ...
                        }
                        else
                        {
                            dynamic dc = t;
                            foreach (var item in dc)
                            {
                                ValidateMode(item);
                            }
                        }
                    }
                    else
                    {
                        dynamic tt = t;
                        foreach (var item in tt)
                        {
                            ValidateMode(item);
                        }
                    }
                    return t;
                }
                else
                {
                    PropertyInfo[] properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    if (properties.Length <= 0)
                    {
                        return default;
                    }

                    for (int i = 0; i < properties.Length; i++)
                    {
                        var modelType = properties[i].PropertyType;
                        if (modelType.Name == "String")
                        {
                            string value = properties[i].GetValue(t, null)?.ToString();
                            if (value == null)
                            {
                                //continue;
                                if (_lstr.Any(item => item == properties[i].Name))
                                {
                                    return default;
                                }
                            }
                            else
                            {
                                if (value != value.Trim())
                                {
                                    string newValue = value.Trim();
                                    if (string.IsNullOrEmpty(newValue))
                                    {
                                        properties[i].SetValue(t, null);
                                    }
                                    else
                                    {
                                        properties[i].SetValue(t, newValue);
                                    }
                                }
                            }
                        }
                        else if (modelType.IsValueType)
                        {
                            // Int* Double Boolean ...
                        }
                        else
                        {
                            properties[i].GetValue(t).ValidateMode();
                        }
                    }
                    return t;
                }
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
