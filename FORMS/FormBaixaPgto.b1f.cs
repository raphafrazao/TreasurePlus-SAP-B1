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

        private string _docEntry; //captura o docentry do contrato!

        public override void OnInitializeComponent()
        {
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_0").Specific));
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_2").Specific));
            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("NumContr").Specific));
            this.EditText0.ChooseFromListAfter += new SAPbouiCOM._IEditTextEvents_ChooseFromListAfterEventHandler(this.EditText0_ChooseFromListAfter);
            this.EditText1 = ((SAPbouiCOM.EditText)(this.GetItem("NamePN").Specific));
            this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("StatusCont").Specific));
            this.EditText3 = ((SAPbouiCOM.EditText)(this.GetItem("CodePN").Specific));
            //           Apague as linhas do Grid0 e Grid1 e coloque isto:
            this.Grid3 = ((SAPbouiCOM.Grid)(this.GetItem("GridParc").Specific));
            this.Grid3.ClickBefore += new SAPbouiCOM._IGridEvents_ClickBeforeEventHandler(this.Grid3_ClickBefore);
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("btnBuscarP").Specific));
            this.Button0.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button0_ClickBefore);
            //     Mapeia o Segundo Grid (Contabilidade Real)
            this.Grid0 = ((SAPbouiCOM.Grid)(this.GetItem("GridContab").Specific));
            this.Grid0.ClickAfter += new SAPbouiCOM._IGridEvents_ClickAfterEventHandler(this.Grid0_ClickAfter);
            this.Grid0.ValidateAfter += new SAPbouiCOM._IGridEvents_ValidateAfterEventHandler(this.Grid0_ValidateAfter);
            this.EditText4 = ((SAPbouiCOM.EditText)(this.GetItem("txtTotPago").Specific));
            this.StaticText3 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_6").Specific));
            this.StaticText4 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_3").Specific));
            this.EditText5 = ((SAPbouiCOM.EditText)(this.GetItem("txtDataPgt").Specific));
            this.Button1 = ((SAPbouiCOM.Button)(this.GetItem("btnBaixar").Specific));
            this.Button1.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button1_ClickBefore);
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
            this.StaticText4.Item.Visible = false;
            this.EditText5.Item.Visible = false;
            this.Button1.Item.Visible = false;

            if (this.Grid0 != null)
            {
                this.Grid0.Item.Visible = false;
            }

            // Preenche o campo Data de Pagamento com a data de hoje no formato do SAP (yyyyMMdd)
            this.UIAPIRawForm.DataSources.UserDataSources.Item("udsDtPgto").ValueEx = DateTime.Now.ToString("yyyyMMdd");

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

                _docEntry = oDataTable.GetValue("DocEntry", 0).ToString();

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

                // =====================================================================
                // A MÁGICA: O "Jeito Melhor" de preencher os juros!
                // Cruzamos o Grid de Baixo (dtContab) com o Grid de Cima (dtParc)
                // =====================================================================
                for (int i = 0; i < dtContab.Rows.Count; i++)
                {
                    string parcelaContabil = dtContab.GetValue("Ref. 2 (Parcela)", i).ToString();

                    // Procura qual é a linha do grid de cima que tem esta mesma parcela
                    for (int j = 0; j < dtParc.Rows.Count; j++)
                    {
                        if (dtParc.GetValue("Parcela", j).ToString() == parcelaContabil)
                        {
                            // Achou! Pega o juro de cima e escreve embaixo
                            double jurosDaParcela = Convert.ToDouble(dtParc.GetValue("Juros", j));
                            dtContab.SetValue("Valor Juros", i, jurosDaParcela);
                            break; // Pula para a próxima linha contábil
                        }
                    }
                }
                // =====================================================================

                // ---> Mostra o Grid já com os dados! <---
                this.Grid0.Item.Visible = true;

                // ---> A MÁGICA ACONTECE AQUI: Mostra o Grid já com os dados! <---
                this.Grid0.Item.Visible = true;
                this.EditText4.Item.Visible = true;
                this.StaticText3.Item.Visible = true;
                this.StaticText4.Item.Visible = true;
                this.EditText5.Item.Visible = true;
                this.Button1.Item.Visible = true;

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

                // Desbloqueia as colunas de Multa e Desconto para digitação
                oGridContab.Columns.Item("Multa").Editable = true;
                oGridContab.Columns.Item("Multa").RightJustified = true;

                oGridContab.Columns.Item("Desconto Juros").Editable = true;
                oGridContab.Columns.Item("Desconto Juros").RightJustified = true;

                oGridContab.Columns.Item("Total Pago").RightJustified = true;

                oGridContab.AutoResizeColumns();
                oGridContab.Item.Visible = true;

                // 6. Calcular e preencher o Campo "Total Pago" (Apague o FOR antigo e use a nova função)

                AtualizarTotalGeral(dtContab);

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



        // -------------------------------------------------------------
        // MÉTODO 1: Recalcula a linha quando o utilizador digita um valor
        // -------------------------------------------------------------
        private void Grid0_ValidateAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                // ==============================================================
                // A TRAVA DE SEGURANÇA: Se o valor não mudou, foge daqui!
                // Isso impede o Loop Infinito e o Crash do SAP B1.
                // ==============================================================
                if (pVal.ItemChanged == false)
                    return;

                // Só recalcula se ele mexeu na Multa ou no Desconto
                if (pVal.ColUID == "Multa" || pVal.ColUID == "Desconto Juros")
                {
                    SAPbouiCOM.Grid oGrid = (SAPbouiCOM.Grid)this.GetItem("GridContab").Specific;
                    int rowIndex = oGrid.GetDataTableRowIndex(pVal.Row);

                    if (rowIndex >= 0)
                    {
                        // Resgata os valores da linha
                        double valorOriginal = Convert.ToDouble(oGrid.DataTable.GetValue("Valor Original (Crédito)", rowIndex));
                        double multa = Convert.ToDouble(oGrid.DataTable.GetValue("Multa", rowIndex));
                        double desconto = Convert.ToDouble(oGrid.DataTable.GetValue("Desconto Juros", rowIndex));

                        // Aplica a regra de negócio: VALOR ORIGINAL + (Multa - Desconto Juros)
                        double totalPago = valorOriginal + (multa - desconto);

                        // Não pode ser menor que zero
                        if (totalPago < 0) totalPago = 0;

                        // Escreve o novo total na linha e recalcula o global da tela
                        oGrid.DataTable.SetValue("Total Pago", rowIndex, totalPago);
                        AtualizarTotalGeral(oGrid.DataTable);
                    }
                }
            }
            catch { }
        }


        // -------------------------------------------------------------
        // MÉTODO 2: Recalcula o Total Geral quando ele marca/desmarca a Checkbox
        // -------------------------------------------------------------
        private void Grid0_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                if (pVal.ColUID == "Baixar" && pVal.Row >= 0)
                {
                    SAPbouiCOM.Grid oGrid = (SAPbouiCOM.Grid)this.GetItem("GridContab").Specific;
                    AtualizarTotalGeral(oGrid.DataTable);
                }
            }
            catch { }

        }

        // -------------------------------------------------------------
        // MÉTODO 3: O Motor que soma tudo e joga no campo txtTotPago
        // -------------------------------------------------------------
        private void AtualizarTotalGeral(SAPbouiCOM.DataTable dt)
        {
            try
            {
                double total = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    // Só soma se a checkbox de "Baixar" estiver flegada (Y)
                    if (dt.GetValue("Baixar", i).ToString() == "Y")
                    {
                        total += Convert.ToDouble(dt.GetValue("Total Pago", i));
                    }
                }

                // Joga o valor formatado para o campo
                this.EditText4.Value = total.ToString("F2");
            }
            catch { }
        }

        private SAPbouiCOM.StaticText StaticText4;
        private SAPbouiCOM.EditText EditText5;
        private SAPbouiCOM.Button Button1;

        private void Button1_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                // 1. Validar Data
                string dtString = ((SAPbouiCOM.EditText)this.GetItem("txtDataPgt").Specific).Value;
                if (string.IsNullOrEmpty(dtString)) throw new Exception("Preencha a Data de Pagamento.");
                DateTime dataPgto = DateTime.ParseExact(dtString, "yyyyMMdd", null);

                // 2. Extrair dados da Grelha Contábil (GridContab)
                SAPbouiCOM.Grid oGridContab = (SAPbouiCOM.Grid)this.GetItem("GridContab").Specific;
                SAPbouiCOM.DataTable dtContab = oGridContab.DataTable;

                List<Dictionary<string, object>> parcelas = new List<Dictionary<string, object>>();

                for (int i = 0; i < dtContab.Rows.Count; i++)
                {
                    if (dtContab.GetValue("Baixar", i).ToString() == "Y")
                    {
                        var info = new Dictionary<string, object>();
                        info["TransId"] = dtContab.GetValue("Nº LCM", i);
                        info["LineId"] = dtContab.GetValue("Linha", i);
                        info["ValorOriginal"] = dtContab.GetValue("Saldo a Pagar", i);
                        info["Multa"] = dtContab.GetValue("Multa", i);
                        info["Desconto"] = dtContab.GetValue("Desconto Juros", i);
                        info["Ref1"] = dtContab.GetValue("Ref. 1 (Contrato)", i);
                        info["Ref2"] = dtContab.GetValue("Ref. 2 (Parcela)", i);

                        // NOVA COLUNA!
                        info["ValorJuros"] = dtContab.GetValue("Valor Juros", i);

                        parcelas.Add(info);
                    }
                }

                if (parcelas.Count == 0) throw new Exception("Não há parcelas marcadas para baixa.");

                this.UIAPIRawForm.Freeze(true);

                // 3. Resgatar as Contas do Contrato Base usando a variável global _docEntry
                if (string.IsNullOrEmpty(_docEntry)) throw new Exception("DocEntry do contrato não carregado. Por favor, selecione novamente.");

                string credorPN = ((SAPbouiCOM.EditText)this.GetItem("CodePN").Specific).Value.Trim();

                SAPbobsCOM.Recordset oRec = (SAPbobsCOM.Recordset)TreasurePlus.CORE.CommomClass.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                // NOVA QUERY SUPER RÁPIDA: Buscando por DocEntry (Chave Primária) em vez do U_NumContrato
                oRec.DoQuery($"SELECT U_BankAcc, U_IntExpAcc, U_SIntAcc FROM [@TP_LOAN] WHERE DocEntry = {_docEntry}");

                if (oRec.RecordCount == 0) throw new Exception("Contrato não encontrado para resgatar contas contábeis.");

                string contaBancaria = oRec.Fields.Item("U_BankAcc").Value.ToString();
                string contaDespesaJuros = oRec.Fields.Item("U_IntExpAcc").Value.ToString();
                string contaJurosApropriar = oRec.Fields.Item("U_SIntAcc").Value.ToString();

                // 4. CHAMAR A CLASSE DE NEGÓCIOS ATUALIZADA (Passando o _docEntry também!)
                TreasurePlus.Business.ContratoBusiness negocio = new TreasurePlus.Business.ContratoBusiness();

                // Nota: Teremos de alterar a assinatura da função na classe de negócios para receber o docEntry
                int idPagamento = negocio.EfetuarBaixaParcelas(Convert.ToInt32(_docEntry), credorPN, contaBancaria, contaDespesaJuros, contaJurosApropriar, dataPgto, parcelas);

                // 5. Sucesso e Limpeza da Tela
                Application.SBO_Application.MessageBox($"Baixa efetuada com sucesso! Pagamento Nº: {idPagamento}", 1, "Ok", "", "");

                // CORREÇÃO DO FOCO: Joga o cursor para o campo "NumContr" (que nunca é escondido)
                this.EditText0.Item.Click();

               
                // 5. Sucesso e Limpeza da Tela
                Application.SBO_Application.MessageBox($"Baixa efetuada com sucesso! Pagamento Nº: {idPagamento}", 1, "Ok", "", "");

                // CORREÇÃO: Recarrega as parcelas do banco de dados (Grid de cima)
                CarregarParcelas(_docEntry);

                // Esconde a grelha de baixo para obrigar o utilizador a fazer uma nova busca
                this.Grid0.Item.Visible = false;
                this.EditText4.Item.Visible = false;
                this.StaticText3.Item.Visible = false;
                this.StaticText4.Item.Visible = false;
                this.EditText5.Item.Visible = false;
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox("Erro na baixa: " + ex.Message, 1, "Ok", "", "");
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }

        }
    }
}
