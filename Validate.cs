 public static class Validate
    {
        private static readonly List<string> _lstr = new List<string>();

        public static T ModelTrim<T>(this T t) where T : class
        {
            return ValidateMode(t);
        }
        public static T ModelTrim<T>(this T t, object ValidateList) where T : class
        {
            if (ValidateList != null)
            {
                PropertyInfo[] properties1 = ValidateList.GetType().GetProperties();
                foreach (var item in properties1)
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
                if (type.Name.StartsWith("String"))
                {
                    //TODO 如何赋值
                    //var a = t.GetType();
                    //var b = a.GetProperty("String");
                    //b.SetValue(t.ToString().Trim(), null);
                }
                PropertyInfo[] properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                if (properties.Length <= 0)
                {
                    return default;
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
                            if (_lstr.Any(item => item == properties[i].Name))
                            {
                                //如果标记的值为空则不予通过
                                return default;
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
                        var aa = properties[i].GetValue(t, null);
                        var bb = aa.GetType();

                        if (bb.GenericTypeArguments.Any())
                        {
                            if (bb.GenericTypeArguments.Count() == 1)
                            {
                                var cc = bb.GenericTypeArguments[0].Name;
                                if (cc.StartsWith("String"))
                                {
                                    dynamic dd = aa;
                                    var ee = dd.ToArray();
                                    var iii = 0;
                                    foreach (var item in ee)
                                    {
                                        dd[iii] = item.Trim();
                                        iii++;
                                    }
                                }
                                else if (cc.StartsWith("Int") || cc.StartsWith("Double") || cc.StartsWith("Boolean"))
                                {

                                }
                                else
                                {
                                    dynamic ff = aa;
                                    foreach (var item in ff)
                                    {
                                        ValidateMode(item);
                                    }
                                }
                            }
                            else
                            {
                                aa.ValidateMode();
                            }
                        }
                        else
                        {
                            aa.ValidateMode();
                        }
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
                        properties[i].GetValue(t).ValidateMode();
                    }
                }

                return t;
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
