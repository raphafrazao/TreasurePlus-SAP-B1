using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreasurePlus
{
    [Form("TreasurePlus.FormPgto", "FORMS/FormBaixaPgto.b1f")]
    class FormPgto : UserFormBase
    {
        public FormPgto()
        {
        }
        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.StaticText StaticText2;
        private SAPbouiCOM.EditText EditText0;
        private SAPbouiCOM.EditText EditText1;
        private SAPbouiCOM.EditText EditText2;
        private SAPbouiCOM.EditText EditText3;
        private SAPbouiCOM.StaticText StaticText1;   
        private SAPbouiCOM.Grid Grid3;
        private SAPbouiCOM.Button Button0;
        private SAPbouiCOM.Grid Grid0;
        private SAPbouiCOM.EditText EditText4;
        private SAPbouiCOM.StaticText StaticText3;

        public override void OnInitializeComponent()
        {
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_0").Specific));
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_2").Specific));
            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("NumContr").Specific));
            this.EditText0.ChooseFromListAfter += new SAPbouiCOM._IEditTextEvents_ChooseFromListAfterEventHandler(this.EditText0_ChooseFromListAfter);
            this.EditText1 = ((SAPbouiCOM.EditText)(this.GetItem("NamePN").Specific));
            this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("StatusCont").Specific));
            this.EditText3 = ((SAPbouiCOM.EditText)(this.GetItem("CodePN").Specific));
            //       Apague as linhas do Grid0 e Grid1 e coloque isto:
            this.Grid3 = ((SAPbouiCOM.Grid)(this.GetItem("GridParc").Specific));
            this.Grid3.ClickBefore += new SAPbouiCOM._IGridEvents_ClickBeforeEventHandler(this.Grid3_ClickBefore);
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("btnBuscarP").Specific));
            this.Button0.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button0_ClickBefore);
            this.Grid0 = ((SAPbouiCOM.Grid)(this.GetItem("GridContab").Specific));
            this.EditText4 = ((SAPbouiCOM.EditText)(this.GetItem("txtTotPago").Specific));
            this.StaticText3 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_6").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            

        }

      

        private void OnCustomInitialize()
        {
            // Inicializa a grelha vazia, apenas para desenhar as colunas!
            CarregarParcelas("0");

            // Esconde a grelha de contabilidade logo que a tela abre
            // (Estou assumindo que você mapeou o GridContab na variável Grid1)
            this.EditText4.Item.Visible = false;
            this.StaticText3.Item.Visible = false;
            if (this.Grid0 != null)
            {
                this.Grid0.Item.Visible = false;
            }

        }

        private void EditText0_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            SAPbouiCOM.ISBOChooseFromListEventArg oCFLEvento = (SAPbouiCOM.ISBOChooseFromListEventArg)pVal;
            SAPbouiCOM.DataTable oDataTable = oCFLEvento.SelectedObjects;

            if (oDataTable == null)
                return;

            try
            {
                // 1. Resgata os valores brutos da lupa (Tabela @TP_LOAN)
                var contrato = oDataTable.GetValue("U_NumContrato", 0).ToString();
                var codePN = oDataTable.GetValue("U_CreditorNumber", 0).ToString();
                var docEntry = oDataTable.GetValue("DocEntry", 0).ToString();

                // O Status nativo do SAP (bboObject) geralmente retorna "O" (Open/1) ou "C" (Closed/2)
                var statusCode = oDataTable.GetValue("Status", 0).ToString();

                // 2. Tradução do Status para o ecrã
                string statusDescricao = "";
                if (statusCode == "1" || statusCode == "O") // "O" de Open ou 1 dependendo da versão
                    statusDescricao = "Aberto";
                else if (statusCode == "2" || statusCode == "C") // "C" de Closed ou 2
                    statusDescricao = "Fechado";
                else if (statusCode == "3" || statusCode == "C") // Cancelado
                    statusDescricao = "Cancelado";
                else
                    statusDescricao = statusCode; // fallback caso seja outro

                // 3. Busca o Nome do Parceiro de Negócios (OCRD) usando o DI API (Recordset)
                string nomePN = "";

                // Verifica e preenche a CommonClass se estiver vazia (igual fizemos no Form1)
                if (TreasurePlus.CORE.CommomClass.oCompany == null)
                {
                    TreasurePlus.CORE.CommomClass.oCompany = (SAPbobsCOM.Company)SAPbouiCOM.Framework.Application.SBO_Application.Company.GetDICompany();
                }

                SAPbobsCOM.Company oCompany = TreasurePlus.CORE.CommomClass.oCompany;
                SAPbobsCOM.Recordset oRec = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                if (!string.IsNullOrEmpty(codePN))
                {
                    oRec.DoQuery($"SELECT CardName FROM OCRD WHERE CardCode = '{codePN}'");
                    if (oRec.RecordCount > 0)
                    {
                        nomePN = oRec.Fields.Item("CardName").Value.ToString();
                    }
                }

                // 4. Preenche a tela (User Data Sources)
                this.UIAPIRawForm.DataSources.UserDataSources.Item("udsContr").ValueEx = contrato;
                this.UIAPIRawForm.DataSources.UserDataSources.Item("udsCredor").ValueEx = codePN;
                this.UIAPIRawForm.DataSources.UserDataSources.Item("udsStatus").ValueEx = statusDescricao; // Passamos o traduzido
                this.UIAPIRawForm.DataSources.UserDataSources.Item("udsNome").ValueEx = nomePN;


                // Exemplo: Se tiver criado o UDS para o nome do fornecedor:
                // this.UIAPIRawForm.DataSources.UserDataSources.Item("udsNomePN").ValueEx = nomePN;

                // DICA: Aqui será o lugar perfeito para chamarmos a função que carrega a Grid de Parcelas!
                // Chama a nossa nova função passando a chave primária!
                CarregarParcelas(docEntry);
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox("Erro ao selecionar contrato: " + ex.Message, 1, "Ok", "", "");
            }
        }




        private void CarregarParcelas(string docEntry)
        {
            try
            {
                // Só congela a tela se ela já estiver visível (evita erros ao abrir)
                if (this.UIAPIRawForm.Visible)
                    this.UIAPIRawForm.Freeze(true);

                // 1. Verifica se a DataTable já existe na tela. Se não, cria-a na hora!
                SAPbouiCOM.DataTable dtParcelas;
                try
                {
                    dtParcelas = this.UIAPIRawForm.DataSources.DataTables.Item("DT_PARC");
                }
                catch
                {
                    dtParcelas = this.UIAPIRawForm.DataSources.DataTables.Add("DT_PARC");
                }

                // 2. A Query mágica (Buscamos apenas as parcelas em aberto na tabela filha)
                // Nota: Assumi que o status de aberto na linha é 'A'. Ajuste se for 'O', '1', etc.
                string query = $@"
                    SELECT 
                        'N' AS 'Selecionar',  /* <-- COLUNA FAKE PARA A CHECKBOX */
                        U_InstNum AS 'Parcela', 
                        U_DueDate AS 'Vencimento', 
                        U_InstAmt AS 'Valor da Parcela', 
                        U_Interest AS 'Juros', 
                        U_Amort AS 'Amortização',
                        U_Status AS 'Status',
                        U_JE_Aprop AS 'LCM Inicial'
                    FROM [@TP_LOAN_LINES] 
                    WHERE DocEntry = {docEntry} 
                      
                    ORDER BY U_InstNum";

                // 3. Executa a query diretamente na DataTable
                dtParcelas.ExecuteQuery(query);

                // 4. Liga a DataTable ao nosso Grid da tela
                SAPbouiCOM.Grid oGrid = (SAPbouiCOM.Grid)this.GetItem("GridParc").Specific;
                oGrid.DataTable = dtParcelas;

                // Pega a coluna "Selecionar"
                SAPbouiCOM.GridColumn colCheck = oGrid.Columns.Item("Selecionar");

                // Transforma a coluna visualmente numa Checkbox
                colCheck.Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox;

                // IMPORTANTE: Esta é a ÚNICA coluna que o utilizador pode editar (clicar)
                colCheck.Editable = true;

                // 5. Ajustes cosméticos do Grid (Proteger o resto contra edição)
                oGrid.Columns.Item("Parcela").Editable = false;
                oGrid.Columns.Item("Vencimento").Editable = false;
                oGrid.Columns.Item("Valor da Parcela").Editable = false;
                oGrid.Columns.Item("Juros").Editable = false;
                oGrid.Columns.Item("Amortização").Editable = false;
                oGrid.Columns.Item("Status").Editable = false;
                oGrid.Columns.Item("LCM Inicial").Editable = false;



                // Ajusta o tamanho das colunas automaticamente
                oGrid.AutoResizeColumns();
                // -------------------------------------------------------------
                // A MÁGICA DAS CORES
                // -------------------------------------------------------------
                int corAmarela = SAPColor(255, 255, 150); // Amarelo suave
                int corVerde = SAPColor(150, 255, 150);   // Verde suave para Pago
                int corVermelha = SAPColor(255, 150, 150); // Vermelho suave para Cancelado

                for (int i = 0; i < dtParcelas.Rows.Count; i++)
                {
                    string statusDaLinha = dtParcelas.GetValue("Status", i).ToString();

                    // No UI API, o índice da linha visual começa em 1
                    int linhaVisual = i + 1;

                    if (statusDaLinha == "A")
                        oGrid.CommonSetting.SetRowBackColor(linhaVisual, corAmarela);
                    else if (statusDaLinha == "P")
                        oGrid.CommonSetting.SetRowBackColor(linhaVisual, corVerde);
                    else if (statusDaLinha == "C")
                        oGrid.CommonSetting.SetRowBackColor(linhaVisual, corVermelha);
                }
                // -------------------------------------------------------------

            }
            catch (Exception ex)
            {
                // Impede que o erro de formatação apareça quando tentamos carregar com "0" e o form ainda está escondido
                if (docEntry != "0")
                    Application.SBO_Application.MessageBox("Erro ao carregar parcelas: " + ex.Message, 1, "Ok", "", "");
            }
            finally
            {
                if (this.UIAPIRawForm.Visible)
                    this.UIAPIRawForm.Freeze(false);
            }
        }
       
        // Função utilitária para converter RGB no padrão numérico do SAP B1
        private int SAPColor(int r, int g, int b)
        {
            return r + (g * 256) + (b * 65536);
        }

        private void Grid3_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true; // Permite o clique por padrão

            try
            {
                // Se clicou na coluna da Checkbox e não foi no cabeçalho (Row >= 0)
                if (pVal.ColUID == "Selecionar" && pVal.Row >= 0)
                {
                    SAPbouiCOM.Grid oGrid = (SAPbouiCOM.Grid)this.GetItem("GridParc").Specific;

                    // Traduz a linha visual onde clicou para a linha real da DataTable
                    int linhaData = oGrid.GetDataTableRowIndex(pVal.Row);

                    if (linhaData >= 0)
                    {
                        string status = oGrid.DataTable.GetValue("Status", linhaData).ToString();

                        // Se for Pago ou Cancelado, BARRAR o clique!
                        if (status == "P" || status == "C")
                        {
                            BubbleEvent = false; // A ação é cancelada, o Vistinho não marca!

                            // Opcional: Mensagem na barra de status
                            SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Não é possível flegar uma parcela que já está Paga ou Cancelada.", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                        }
                    }
                }
            }
            catch { }

        }

        private void Button0_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                SAPbouiCOM.Grid oGridParc = (SAPbouiCOM.Grid)this.GetItem("GridParc").Specific;
                SAPbouiCOM.DataTable dtParc = oGridParc.DataTable;

                // 1. Descobrir quais parcelas estão marcadas ("Y") na interface
                List<string> parcelasSelecionadas = new List<string>();
                for (int i = 0; i < dtParc.Rows.Count; i++)
                {
                    if (dtParc.GetValue("Selecionar", i).ToString() == "Y")
                    {
                        parcelasSelecionadas.Add(dtParc.GetValue("Parcela", i).ToString());
                    }
                }

                if (parcelasSelecionadas.Count == 0)
                    throw new Exception("Por favor, flegue (selecione) pelo menos uma parcela no grid acima.");

                this.UIAPIRawForm.Freeze(true);

                // 2. Extrair dados da tela
                string contratoExterno = ((SAPbouiCOM.EditText)this.GetItem("NumContr").Specific).Value.Trim();
                string credorPN = ((SAPbouiCOM.EditText)this.GetItem("CodePN").Specific).Value.Trim();

                // -------------------------------------------------------------
                // 3. CHAMADA À CLASSE DE NEGÓCIOS (CLEAN CODE!)
                // -------------------------------------------------------------
                TreasurePlus.Business.ContratoBusiness negocioContrato = new TreasurePlus.Business.ContratoBusiness();
                string queryContab = negocioContrato.ObterQueryPendenciasContabeis(credorPN, contratoExterno, parcelasSelecionadas);

                // 4. Executar a Query na DataTable
                SAPbouiCOM.DataTable dtContab;
                try { dtContab = this.UIAPIRawForm.DataSources.DataTables.Item("DT_CONTAB"); }
                catch { dtContab = this.UIAPIRawForm.DataSources.DataTables.Add("DT_CONTAB"); }

                dtContab.ExecuteQuery(queryContab);

                // ---> A MÁGICA ACONTECE AQUI: Mostra o Grid já com os dados! <---
                this.Grid0.Item.Visible = true;
                this.EditText4.Item.Visible = true;
                this.StaticText3.Item.Visible = true;

                // 5. Vincular ao Grid de baixo e formatar
                SAPbouiCOM.Grid oGridContab = (SAPbouiCOM.Grid)this.GetItem("GridContab").Specific;
                oGridContab.DataTable = dtContab;

                // Transforma a coluna 'Baixar' em Checkbox
                SAPbouiCOM.GridColumn colCheckC = oGridContab.Columns.Item("Baixar");
                colCheckC.Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox;
                colCheckC.Editable = true;

                // Cria Links nativos
                ((SAPbouiCOM.EditTextColumn)oGridContab.Columns.Item("Nº LCM")).LinkedObjectType = "30";
                ((SAPbouiCOM.EditTextColumn)oGridContab.Columns.Item("Credor")).LinkedObjectType = "2";

                // Proteger as outras colunas
                for (int i = 1; i < oGridContab.Columns.Count; i++)
                {
                    oGridContab.Columns.Item(i).Editable = false;
                }
                oGridContab.AutoResizeColumns();

                // 6. Calcular e preencher o Campo "Total Pago" (Via Interface)
                double totalContabil = 0;
                for (int i = 0; i < dtContab.Rows.Count; i++)
                {
                    totalContabil += Convert.ToDouble(dtContab.GetValue("Saldo a Pagar", i));
                }

                this.EditText4.Value = totalContabil.ToString("F2"); // Assumindo que mapeou txtTotPago como EditText4

                SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Pendências contábeis localizadas com sucesso!", SAPbouiCOM.BoMessageTime.bmt_Short, false);
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox("Erro ao buscar: " + ex.Message, 1, "Ok", "", "");
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }

        }
    }
}
