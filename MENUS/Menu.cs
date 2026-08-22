using SAPbouiCOM.Framework;
using System;

namespace TreasurePlus
{
    class Menu
    {
        private const string MenuUid = "TreasurePlus_Folder";
        private const string FormMenuUid = "TreasurePlus.Form1";

        public void AddMenuItems()
        {
            string xmlMenu = RESOURCES.Resource.MenuAdd;

            try
            {
                Application.SBO_Application.LoadBatchActions(xmlMenu);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Não foi possível criar os menus do TreasurePlus.",
                    ex);
            }
        }

        public void RemoverMenuSeExistir()
        {
            try
            {
                // Consulta o menu pelo UID.
                Application.SBO_Application.Menus.Item(MenuUid);

                // Se encontrou, remove a estrutura XML.
                string xmlMenu = RESOURCES.Resource.MenuRemove;
                Application.SBO_Application.LoadBatchActions(xmlMenu);
            }
            catch
            {
                // Neste método, a ausência do menu é considerada normal.
                // Durante a implantação, registre o erro em arquivo se necessário.
            }
        }

        public void SBO_Application_MenuEvent(
            ref SAPbouiCOM.MenuEvent pVal,
            out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                // O formulário deve ser aberto após o SAP concluir a ação do menu.
                if (pVal.BeforeAction)
                    return;

                // O clique ocorre no último item da árvore, não na pasta-pai.
                if (pVal.MenuUID != FormMenuUid)
                    return;

                var activeForm = new Form1();
                activeForm.Show();
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(
                    "Erro ao abrir a tela: " + ex.Message,
                    1,
                    "Ok",
                    "",
                    "");
            }
        }
    }
}

