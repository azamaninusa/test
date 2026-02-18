using EdiFabric;
using NightlyBillingData.DbModels;
using System.Collections;
using System.Reflection;
using VaxCare.EdiFabric.Templates.Hipaa5010;
using VaxCare.Edi.Transactions.Claims;
using VaxCare.Edi.Contracts.Claims;

namespace NightlyBillingData
{
    public class EDI837MessageExtractor
    {
        static EDI837MessageExtractor()
        {
            SerialKey.Set("f34f14b7df5149bd89a7e8cd5bba8f4f");
        }

        public string Get837Message(MessageOut messageOut)
        {
            var messageData = messageOut.MessageData;
            return messageData;
        }

        public ClaimForm Get837MessageObject(string fullMessage)
        {
            var messageObject = ClaimMessageParser.GenerateClaimForm(fullMessage);
            return messageObject;
        }

        public TS837P CreateEdiTransactionFromClaimMessage(string fullMessage)
        {
            const string header =
                "ISA*00*          *00*          *ZZ*TestSenderId   *ZZ*TestClearinghouse       *200707*1033*^*00501*011234123*0*P*:~GS*HC*TestSenderId*TestClearinghouse       *20200707*1033*11234123*X*005010X222A1~";
            const string footer = "GE*1*11234123~IEA*1*011234123~";

            var transactionMessage = $"{header}{fullMessage}{footer}";

            var messages = EDIMessageParser.Parse837Message(transactionMessage);

            return messages.FirstOrDefault() ?? new TS837P();
        }

        public List<ClaimSegments> GetAllSegments<T>(T val, string fullPath = "")
        {
            var segments = new List<ClaimSegments>();
            var fields = GetPublicProperties(val!.GetType());
            foreach (var f in fields)
            {
                if (string.IsNullOrWhiteSpace(fullPath))
                {
                    fullPath += f.Name;
                }
                else
                {
                    fullPath += "." + f.Name;
                } 
                var valA = f.GetValue(val);

                if (f.PropertyType != typeof(string) && f.PropertyType != typeof(int) && valA is not null)
                {
                    var subSegments = new List<ClaimSegments>();
                    if (f.PropertyType.IsGenericType &&
                        f.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var listA = (IList)valA;
                        var itemCount = listA.Count;
                        switch (itemCount)
                        {
                            case 1:
                                {
                                    var listVal = listA[0];
                                    subSegments.AddRange(GetAllSegments(listVal, fullPath));
                                    break;
                                }
                            case > 0:
                                {
                                    for (var i = 0; i < itemCount; i++)
                                    {
                                        var listVal = listA[i];
                                        subSegments.AddRange(GetAllSegments(listVal, fullPath));
                                    }

                                    break;
                                }
                        }
                    }
                    else
                    {
                        subSegments.AddRange(GetAllSegments(valA, fullPath));
                    }
                    if (subSegments.Any())
                    {
                        segments.AddRange(subSegments);
                    }
                }
                else
                {
                    var propName = f.Name;
                    
                    var v = new ClaimSegments
                    {
                        Name = fullPath,
                        SegmentValue = valA?.ToString() ?? string.Empty
                    };
                    segments.Add(v);
                }
            }
            return segments;
        }

        public PropertyInfo[] GetPublicProperties(Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var typeToParse = new List<Type>();
                var queue = new Queue<Type>();
                typeToParse.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (typeToParse.Contains(subInterface))
                        {
                            continue;
                        }

                        typeToParse.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(
                        BindingFlags.FlattenHierarchy
                        | BindingFlags.Public
                        | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(
                BindingFlags.FlattenHierarchy
                | BindingFlags.Public | BindingFlags.Instance);
        }
    }
}
