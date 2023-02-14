using System;
using System.IO;
using System.Security;
using System.Security.Permissions;

namespace AppDomainExample
{
    class Program
    {
        static void Main()
        {
            PermissionSet permSet = new PermissionSet(PermissionState.None);

            permSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.NoAccess, $@"C:\Users\{Environment.UserName}\Desktop"));
            permSet.AddPermission(new EnvironmentPermission(PermissionState.Unrestricted));
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));


            var appsetup = new AppDomainSetup();
            appsetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            AppDomain appDomain = AppDomain.CreateDomain("secured", null, appsetup, permSet);

            var typeThirdParty = typeof(ThirdParty);

            try
            {
                appDomain.CreateInstance(typeThirdParty.Assembly.FullName, typeThirdParty.FullName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                AppDomain.Unload(appDomain);
            }


            new A();
            new B();
        }
    }





    //[Serializable]
    class ThirdParty
    {
        public ThirdParty()
        {
            Console.WriteLine("Load");
            File.Create($@"C:\Users\{Environment.UserName}\Desktop\xxx.txt");
        }

        ~ThirdParty()
        {
            Console.WriteLine("Unload");
        }
    }

    class A { }

    class B { }
}
