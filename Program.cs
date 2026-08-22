using SAPbouiCOM.Framework;
using System;

namespace TreasurePlus
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application oApp;

                if (args.Length < 1)
                    oApp = new Application();
                else
                    oApp = new Application(args[0]);

                var menuManager = new Menu();

                // Remove a versão anterior, se existir.
                menuManager.RemoverMenuSeExistir();

                // Cria a versão atual do menu.
                menuManager.AddMenuItems();

                // Registra o evento de clique dos menus.
                oApp.RegisterMenuEventHandler(
                    menuManager.SBO_Application_MenuEvent);

                // Registra eventos gerais da aplicação SAP.
                Application.SBO_Application.AppEvent +=
                    new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(
                        SBO_Application_AppEvent);

                Application.SBO_Application.StatusBar.SetText(
                    "TreasurePlus conectado com sucesso.",
                    SAPbouiCOM.BoMessageTime.bmt_Short,
                    SAPbouiCOM.BoStatusBarMessageType.smt_Success);

                // Mantém o add-on aguardando eventos.
                oApp.Run();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    "Erro ao iniciar o add-on TreasurePlus:\n\n" + ex,
                    "TreasurePlus",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        static void SBO_Application_AppEvent(
            SAPbouiCOM.BoAppEventTypes eventType)
        {
            switch (eventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                    System.Windows.Forms.Application.Exit();
                    break;

                case SAPbouiCOM.BoAppEventTypes.aet_FontChanged:
                case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                    // Normalmente não é necessário encerrar.
                    break;
            }
        }
    }
}