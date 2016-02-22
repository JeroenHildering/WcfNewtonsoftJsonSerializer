using System;
using System.ServiceModel.Configuration;

namespace WcfNewtonsoftJsonSerializer
{
    public class NewtonsoftJsonBehaviorExtension : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof ( NewtonsoftJsonBehavior ); }
        }

        protected override object CreateBehavior()
        {
            return new NewtonsoftJsonBehavior();
        }
    }
}