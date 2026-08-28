using SAPbouiCOM.Framework;
using System;

namespace TreasurePlus
{
    class Menu
    {
        private const string MenuUid = "TreasurePlus_Folder";
        private const string FormMenuUid = "TreasurePlus.Form1";

        // 1. Criamos a constante para o ID do novo botão
        private const string FormPgtoMenuUid = "TreasurePlus.FormPgto";

        public void AddMenuItems()
        {
            string xmlMenu = RESOURCES.Resource.MenuAdd;
            try
            {
                Application.SBO_Application.LoadBatchActions(xmlMenu);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Não foi possível criar os menus do TreasurePlus.", ex);
            }
        }

        public void RemoverMenuSeExistir()
        {
            try
            {
                Application.SBO_Application.Menus.Item(MenuUid);
                string xmlMenu = RESOURCES.Resource.MenuRemove;
                Application.SBO_Application.LoadBatchActions(xmlMenu);
            }
            catch { }
        }

        public void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                if (pVal.BeforeAction)
                    return;

                // 2. Trocamos o IF por um SWITCH para organizar as chamadas de telas
                switch (pVal.MenuUID)
                {
                    case FormMenuUid:
                        var activeForm = new FormContrato();
                        activeForm.Show();
                        break;

                    
                    case FormPgtoMenuUid:
                        // Use o novo nome da classe que o B1 Studio gerou!
                        var formBaixa = new FormPgto();
                        formBaixa.Show();
                        break;
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox("Erro ao abrir a tela: " + ex.Message, 1, "Ok", "", "");
            }
        }
    }
}