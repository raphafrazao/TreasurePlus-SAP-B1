using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbobsCOM;
using TreasurePlus.CORE;


namespace TreasurePlus.Business
{
    public class ContratoBusiness
    {
        // 1. Puxa a conexão global da DI API automaticamente
        //private Company oCompany = TreasurePlus.CORE.CommomClass.oCompany;

        private Company oCompany = CommomClass.oCompany;

        // =======================================================
        // MÉTODO 1: GERAR LANÇAMENTO CONTÁBIL
        // =======================================================
        public int GerarLancamentoContabilContrato(
            string contratoId, string codePN, double vpl, double valorIof,
            DateTime dtIni, DateTime dtFim, SAPbouiCOM.DataTable oGridData,
            string contaBancaria, string contaDespIof, string contaCp,
            string contaLp, string contaJurosCp, string contaJurosLp)
        {
            SAPbobsCOM.JournalEntries oJE = null;
            try
            {
                oJE = (SAPbobsCOM.JournalEntries)oCompany.GetBusinessObject(
                    SAPbobsCOM.BoObjectTypes.oJournalEntries);

                oJE.TaxDate = dtIni;
                oJE.DueDate = dtFim;
                oJE.ReferenceDate = dtIni;
                oJE.Memo = "Contrato de Emprestimo " + contratoId + " " + codePN;

                int totalParcelas = oGridData.Rows.Count;

                if (vpl > 0)
                {
                    oJE.Lines.AccountCode = contaBancaria;
                    oJE.Lines.Debit = vpl;
                    oJE.Lines.TaxDate = dtIni;
                    oJE.Lines.DueDate = dtIni;
                    oJE.Lines.LineMemo = "Contrato de Emprestimo " + contratoId +
                                         " " + codePN + " - Aporte Bruto";
                    oJE.Lines.Add();
                }

                if (valorIof > 0)
                {
                    oJE.Lines.AccountCode = contaDespIof;
                    oJE.Lines.Debit = valorIof;
                    oJE.Lines.TaxDate = dtIni;
                    oJE.Lines.DueDate = dtIni;
                    oJE.Lines.LineMemo = "Contrato de Emprestimo " + contratoId +
                                         " " + codePN + " - Despesa IOF";
                    oJE.Lines.Add();

                    oJE.Lines.AccountCode = contaBancaria;
                    oJE.Lines.Credit = valorIof;
                    oJE.Lines.TaxDate = dtIni;
                    oJE.Lines.DueDate = dtIni;
                    oJE.Lines.LineMemo = "Contrato de Emprestimo " + contratoId +
                                         " " + codePN + " - IOF Debitado em Conta";
                    oJE.Lines.Add();
                }

                for (int i = 0; i < totalParcelas; i++)
                {
                    int numParcela = Convert.ToInt32(oGridData.GetValue("Parc", i));
                    DateTime vencimento = Convert.ToDateTime(oGridData.GetValue("Venc", i));
                    double valorParcela = Convert.ToDouble(oGridData.GetValue("VlParc", i));
                    double juros = Convert.ToDouble(oGridData.GetValue("Juros", i));
                    bool isCurtoPrazo = vencimento <= dtIni.AddMonths(12);
                    string contaJurosUsada = isCurtoPrazo ? contaJurosCp : contaJurosLp;
                    string contaPassivoUsada = isCurtoPrazo ? contaCp : contaLp;

                    if (juros > 0)
                    {
                        oJE.Lines.AccountCode = contaJurosUsada;
                        oJE.Lines.Debit = juros;
                        oJE.Lines.TaxDate = dtIni;
                        oJE.Lines.DueDate = vencimento;

                        // O SEGREDO AQUI: Guardando os vínculos nativamente!
                        oJE.Lines.Reference1 = contratoId;           // Ref 1 = ID do Contrato
                        oJE.Lines.Reference2 = numParcela.ToString(); // Ref 2 = Número da Parcela
                                                                      // Insere a Flag de Bloqueio para o Contas a Pagar                       
                        oJE.Lines.AdditionalReference = "Contrato TreasurePlus"; // "Reference3", 
                        oJE.Lines.LineMemo = "Contrato de Emprestimo " + contratoId +
                                             " " + codePN + " " + numParcela + "/" + totalParcelas;
                        oJE.Lines.Add();
                    }

                    if (valorParcela > 0)
                    {
                        oJE.Lines.AccountCode = contaPassivoUsada;
                        oJE.Lines.ShortName = codePN;
                        oJE.Lines.Credit = valorParcela;
                        oJE.Lines.TaxDate = dtIni;
                        oJE.Lines.DueDate = vencimento;
                        // O SEGREDO AQUI TAMBÉM:
                        oJE.Lines.Reference1 = contratoId;// Ref 1 = ID do Contrato
                        oJE.Lines.Reference2 = numParcela.ToString();// Ref 2 = Número da Parcela
                        oJE.Lines.AdditionalReference = "Contrato TreasurePlus"; // "Reference3", 
                        oJE.Lines.LineMemo = "Contrato de Emprestimo " + contratoId +
                                             " " + codePN + " " + numParcela + "/" + totalParcelas;
                        oJE.Lines.Add();
                    }
                }

                if (oJE.Add() != 0)
                    throw new Exception("Erro LCM: " + oCompany.GetLastErrorDescription());

                return Convert.ToInt32(oCompany.GetNewObjectKey());
            }
            finally
            {
                if (oJE != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oJE);
            }
        }

        // =======================================================
        // MÉTODO 2: SALVAR O UDO (CABEÇALHO E LINHAS)
        // =======================================================
        public void SalvarContratoNoUDO(
            string contratoId, string codePN, double vpl, double valorIof,
            double taxa, int parcelas, string metodo, DateTime dtIni,
            DateTime dtFim, SAPbouiCOM.DataTable oGridData, int transIdContabil,
            string contaBancaria, string contaCp, string contaLp,
            string contaJurosCp, string contaJurosLp, string contaDespJuros,
            string contaDespIof)
        {
            SAPbobsCOM.CompanyService oCompanyService = oCompany.GetCompanyService();
            SAPbobsCOM.GeneralService oGeneralService = oCompanyService.GetGeneralService("TP_LOAN");
            SAPbobsCOM.GeneralData oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService
                .GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);

            oGeneralData.SetProperty("U_NumContrato", contratoId);
            oGeneralData.SetProperty("U_CreditorNumber", codePN);
            oGeneralData.SetProperty("U_StartDate", dtIni);
            oGeneralData.SetProperty("U_EndDate", dtFim);
            oGeneralData.SetProperty("U_FinancedAmount", vpl);
            oGeneralData.SetProperty("U_IOFValue", valorIof);
            oGeneralData.SetProperty("U_Rate", taxa);
            oGeneralData.SetProperty("U_Install", parcelas);
            oGeneralData.SetProperty("U_AmortMet", metodo);
            oGeneralData.SetProperty("U_BankAcc", contaBancaria);
            oGeneralData.SetProperty("U_ShortTAcc", contaCp);
            oGeneralData.SetProperty("U_LongTAcc", contaLp);
            oGeneralData.SetProperty("U_LIntAcc", contaJurosLp);
            oGeneralData.SetProperty("U_SIntAcc", contaJurosCp);
            oGeneralData.SetProperty("U_IntExpAcc", contaDespJuros);
            oGeneralData.SetProperty("U_IOFExpAcc", contaDespIof);
            oGeneralData.SetProperty("U_Status", 1);

            SAPbobsCOM.GeneralDataCollection oChildren = oGeneralData.Child("TP_LOAN_LINES");

            for (int i = 0; i < oGridData.Rows.Count; i++)
            {
                SAPbobsCOM.GeneralData oChild = oChildren.Add();
                // CORRIGIDO PARA OS IDs EXATOS DA SUA DATATABLE:
                oChild.SetProperty("U_InstNum", oGridData.GetValue("Parc", i));
                oChild.SetProperty("U_DueDate", oGridData.GetValue("Venc", i));
                oChild.SetProperty("U_InstAmt", oGridData.GetValue("VlParc", i));
                oChild.SetProperty("U_Interest", oGridData.GetValue("Juros", i));
                oChild.SetProperty("U_Amort", oGridData.GetValue("Amort", i));
                oChild.SetProperty("U_Status", "A");
                oChild.SetProperty("U_JE_Aprop", transIdContabil);
            }

            oGeneralService.Add(oGeneralData);
        }

        // =======================================================
        //  MÉTODO 3: RETORNA O VALOR DAS PARCELAS SELECIONADAS AO GRID DE BAIXA DE PARCELAS 
        // =======================================================
        public string ObterQueryPendenciasContabeis(string credorPN, string contratoExterno, List<string> parcelasSelecionadas)
        {
            if (parcelasSelecionadas == null || parcelasSelecionadas.Count == 0)
                throw new Exception("Nenhuma parcela selecionada para busca.");

            // Formata os números das parcelas para o SQL (ex: '1','2','3')
            string parcelasFiltroSql = "'" + string.Join("','", parcelasSelecionadas) + "'";

            // A MEGA QUERY CONTÁBIL (Ref1 = Contrato | Ref2 = Parcela | Ref3 = Contrato TreasurePlus)
            string queryContab = "";

            if (oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
            {
                // HANA: Query super limpa, apenas tabelas nativas
                queryContab = $@"
                    SELECT 
                        'Y' AS ""Baixar"", 
                        T0.""TransId"" AS ""Nº LCM"", 
                        T1.""Line_ID"" AS ""Linha"",
                        T1.""ShortName"" AS ""Credor"", 
                        T1.""Account"" AS ""Conta Controle"", 
                        T1.""Ref1"" AS ""Ref. 1 (Contrato)"", 
                        T1.""Ref2"" AS ""Ref. 2 (Parcela)"", 
                        T1.""Ref3Line"" AS ""Ref. 3 (Origem)"",
                        T1.""DueDate"" AS ""Vencimento"", 
                        T1.""Credit"" AS ""Valor Original (Crédito)"",
                        T1.""BalDueCred"" AS ""Saldo a Pagar"",
                        0.0 AS ""Multa"",
                        0.0 AS ""Desconto Juros"",
                        T1.""Credit"" AS ""Total Pago"",
                        0.0 AS ""Valor Juros"" 
                    FROM ""OJDT"" T0
                    INNER JOIN ""JDT1"" T1 ON T0.""TransId"" = T1.""TransId""
                    WHERE T1.""ShortName"" = '{credorPN}'
                      AND T1.""Ref1"" = '{contratoExterno}'
                      AND T1.""Ref2"" IN ({parcelasFiltroSql})
                      AND T1.""Ref3Line"" = 'Contrato TreasurePlus'
                      AND T1.""BalDueCred"" > 0 
                    ORDER BY T1.""Ref2""";
            }
            else
            {
                // SQL SERVER: Query super limpa, apenas tabelas nativas
                queryContab = $@"
                    SELECT 
                        'Y' AS [Baixar], 
                        T0.TransId AS [Nº LCM], 
                        T1.Line_ID AS [Linha],
                        T1.ShortName AS [Credor], 
                        T1.Account AS [Conta Controle], 
                        T1.Ref1 AS [Ref. 1 (Contrato)], 
                        T1.Ref2 AS [Ref. 2 (Parcela)], 
                        T1.Ref3Line AS [Ref. 3 (Origem)],
                        T1.DueDate AS [Vencimento], 
                        T1.Credit AS [Valor Original (Crédito)],
                        T1.BalDueCred AS [Saldo a Pagar],
                        0.0 AS [Multa],
                        0.0 AS [Desconto Juros],
                        T1.Credit AS [Total Pago],
                        0.0 AS [Valor Juros] 
                    FROM OJDT T0
                    INNER JOIN JDT1 T1 ON T0.TransId = T1.TransId
                    WHERE T1.ShortName = '{credorPN}'
                      AND T1.Ref1 = '{contratoExterno}'
                      AND T1.Ref2 IN ({parcelasFiltroSql})
                      AND T1.Ref3Line = 'Contrato TreasurePlus'
                      AND T1.BalDueCred > 0 
                    ORDER BY T1.Ref2";
            }

            return queryContab;
        }

            // =======================================================

            //MÉTODO 4:Pagamento + LCM de Apropriação 
            // =======================================================


            public int EfetuarBaixaParcelas(
            int docEntryContrato, // NOVO PARÂMETRO DA CHAVE PRIMÁRIA
            string credorCode,
            string contaBancaria,
            string contaDespesasJuros,
            string contaJurosApropriar, /* NOVO PARÂMETRO */
            DateTime dataPagamento,
            List<Dictionary<string, object>> parcelasParaBaixar)
        {
            SAPbobsCOM.Company oCompany = TreasurePlus.CORE.CommomClass.oCompany;

            SAPbobsCOM.Payments oPayment = (SAPbobsCOM.Payments)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oVendorPayments);
            SAPbobsCOM.JournalEntries oJE = (SAPbobsCOM.JournalEntries)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);

            try
            {
                // INICIA A TRANSAÇÃO: Se falhar o LCM, o Pagamento é cancelado e vice-versa
                if (!oCompany.InTransaction) oCompany.StartTransaction();

                // =================================================================
                // 1. GERAR O LCM DE APROPRIAÇÃO (JUROS, MULTAS E DESCONTOS) PRIMEIRO!
                // =================================================================
                int idLCMGerado = 0;
                bool temLinhaNoLCM = false;

                foreach (var p in parcelasParaBaixar)
                {
                    double valorJuros = Convert.ToDouble(p["ValorJuros"]);
                    double multa = Convert.ToDouble(p["Multa"]);
                    double desconto = Convert.ToDouble(p["Desconto"]);

                    // --- A. LANÇAMENTO DOS JUROS ---
                    if (valorJuros > 0)
                    {
                        if (temLinhaNoLCM) oJE.Lines.Add();
                        oJE.Lines.AccountCode = contaDespesasJuros;
                        oJE.Lines.Debit = valorJuros;
                        oJE.Lines.Reference1 = p["Ref1"].ToString();
                        oJE.Lines.Reference2 = p["Ref2"].ToString();
                        oJE.Lines.Add();

                        oJE.Lines.AccountCode = contaJurosApropriar;
                        oJE.Lines.Credit = valorJuros;
                        oJE.Lines.Reference1 = p["Ref1"].ToString();
                        oJE.Lines.Reference2 = p["Ref2"].ToString();
                        temLinhaNoLCM = true;
                    }

                    // --- B. LANÇAMENTO DA MULTA (Cria Dívida no Fornecedor) ---
                    if (multa > 0)
                    {
                        if (temLinhaNoLCM) oJE.Lines.Add();
                        oJE.Lines.AccountCode = contaDespesasJuros;
                        oJE.Lines.Debit = multa;
                        oJE.Lines.Reference1 = p["Ref1"].ToString();
                        oJE.Lines.Reference2 = p["Ref2"].ToString();
                        oJE.Lines.Add();

                        oJE.Lines.ShortName = credorCode; // Crédito no fornecedor (Aumenta o que devemos)
                        oJE.Lines.Credit = multa;
                        oJE.Lines.Reference1 = p["Ref1"].ToString();
                        oJE.Lines.Reference2 = p["Ref2"].ToString();
                        temLinhaNoLCM = true;
                    }

                    // --- C. LANÇAMENTO DO DESCONTO (Cria Crédito no Fornecedor) ---
                    if (desconto > 0)
                    {
                        if (temLinhaNoLCM) oJE.Lines.Add();
                        oJE.Lines.ShortName = credorCode; // Débito no fornecedor (Reduz o que devemos)
                        oJE.Lines.Debit = desconto;
                        oJE.Lines.Reference1 = p["Ref1"].ToString();
                        oJE.Lines.Reference2 = p["Ref2"].ToString();
                        oJE.Lines.Add();

                        oJE.Lines.AccountCode = contaDespesasJuros;
                        oJE.Lines.Credit = desconto;
                        oJE.Lines.Reference1 = p["Ref1"].ToString();
                        oJE.Lines.Reference2 = p["Ref2"].ToString();
                        temLinhaNoLCM = true;
                    }
                }

                if (temLinhaNoLCM)
                {
                    oJE.ReferenceDate = dataPagamento;
                    oJE.TaxDate = dataPagamento;
                    oJE.DueDate = dataPagamento;
                    oJE.Memo = "Apropriação e Ajustes - Baixa TreasurePlus";

                    if (oJE.Add() != 0)
                        throw new Exception("Erro na Apropriação do LCM: " + oCompany.GetLastErrorDescription());

                    idLCMGerado = Convert.ToInt32(oCompany.GetNewObjectKey());
                }

                // =================================================================
                // 2. GERAR O PAGAMENTO (LIQUIDANDO AS DÍVIDAS E AS MULTAS JUNTAS)
                // =================================================================
                oPayment.DocType = SAPbobsCOM.BoRcptTypes.rSupplier;
                oPayment.CardCode = credorCode;
                oPayment.DocDate = dataPagamento;
                oPayment.TaxDate = dataPagamento;
                oPayment.JournalRemarks = "Baixa de Empréstimo TreasurePlus";

                double totalFinalPagar = 0;
                bool primeiraLinhaPagto = true;

                // 2.1 Adiciona as Dívidas Originais ao Pagamento
                foreach (var p in parcelasParaBaixar)
                {
                    double valorOriginal = Convert.ToDouble(p["ValorOriginal"]);

                    if (!primeiraLinhaPagto) oPayment.Invoices.Add();
                    oPayment.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_JournalEntry;
                    oPayment.Invoices.DocEntry = Convert.ToInt32(p["TransId"]);
                    oPayment.Invoices.DocLine = Convert.ToInt32(p["LineId"]);
                    oPayment.Invoices.SumApplied = valorOriginal;

                    totalFinalPagar += valorOriginal;
                    primeiraLinhaPagto = false;
                }

                // 2.2 Busca as Multas/Descontos do LCM recém-criado e adiciona ao Pagamento
                if (idLCMGerado > 0)
                {
                    SAPbobsCOM.Recordset oRecLCM = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    // Busca apenas as linhas que afetam o Fornecedor dentro do LCM que acabámos de criar
                    oRecLCM.DoQuery($"SELECT Line_ID, Credit, Debit FROM JDT1 WHERE TransId = {idLCMGerado} AND ShortName = '{credorCode}'");

                    while (!oRecLCM.EoF)
                    {
                        int lineId = Convert.ToInt32(oRecLCM.Fields.Item("Line_ID").Value);
                        double credit = Convert.ToDouble(oRecLCM.Fields.Item("Credit").Value);
                        double debit = Convert.ToDouble(oRecLCM.Fields.Item("Debit").Value);

                        if (!primeiraLinhaPagto) oPayment.Invoices.Add();
                        oPayment.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_JournalEntry;
                        oPayment.Invoices.DocEntry = idLCMGerado;
                        oPayment.Invoices.DocLine = lineId;

                        if (credit > 0) // É uma Multa (O fornecedor está a cobrar-nos mais)
                        {
                            oPayment.Invoices.SumApplied = credit;
                            totalFinalPagar += credit;
                        }
                        else if (debit > 0) // É um Desconto (Temos crédito com o fornecedor)
                        {
                            oPayment.Invoices.SumApplied = -debit; // Desconto entra negativo no pagamento
                            totalFinalPagar -= debit;
                        }

                        primeiraLinhaPagto = false;
                        oRecLCM.MoveNext();
                    }
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecLCM);
                }

                if (totalFinalPagar < 0) totalFinalPagar = 0;

                oPayment.TransferAccount = contaBancaria;
                oPayment.TransferSum = totalFinalPagar;
                oPayment.TransferDate = dataPagamento;

                if (oPayment.Add() != 0) throw new Exception("Erro no Pagamento: " + oCompany.GetLastErrorDescription());
                int idPagamentoGerado = Convert.ToInt32(oCompany.GetNewObjectKey());
                // =================================================================
                // 3. ATUALIZAR A TABELA DO ADD-ON VIA UDO
                // =================================================================

                // Não precisamos mais fazer SELECT para achar o DocEntry, ele já foi passado por parâmetro!
                if (docEntryContrato > 0)
                {
                    // 3.2 Prepara os serviços do UDO
                    SAPbobsCOM.CompanyService oCompService = oCompany.GetCompanyService();
                    SAPbobsCOM.GeneralService oGeneralService = oCompService.GetGeneralService("TP_LOAN");
                    SAPbobsCOM.GeneralDataParams oGeneralParams = (SAPbobsCOM.GeneralDataParams)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);

                    // 3.3 Carrega o Objeto do Contrato para a memória
                    oGeneralParams.SetProperty("DocEntry", docEntryContrato); // USA A VARIÁVEL DIRETO!

                    SAPbobsCOM.GeneralData oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                    SAPbobsCOM.GeneralDataCollection oChildren = oGeneralData.Child("TP_LOAN_LINES");

                    // 3.4 Percorre as parcelas baixadas e encontra a linha correspondente no UDO
                    foreach (var p in parcelasParaBaixar)
                    {
                        string numParcelaBaixada = p["Ref2"].ToString();

                        for (int i = 0; i < oChildren.Count; i++)
                        {
                            SAPbobsCOM.GeneralData oChild = oChildren.Item(i);
                            string instNumDaLinha = oChild.GetProperty("U_InstNum").ToString();

                            // Achou a linha exata da parcela!
                            if (instNumDaLinha == numParcelaBaixada)
                            {
                                oChild.SetProperty("U_Status", "P");
                                oChild.SetProperty("U_PayDoc", idPagamentoGerado);

                                if (idLCMGerado > 0)
                                    oChild.SetProperty("U_JE_Aprop", idLCMGerado);

                                break;
                            }
                        }
                    }

                    // 3.5 Grava o UDO de volta na base de dados
                    oGeneralService.Update(oGeneralData);
                }

                if (oCompany.InTransaction) oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);

                return idPagamentoGerado;

            }
            catch (Exception)
            {
                // SE DEU ERRO EM QUALQUER LUGAR, DESFAZ TUDO (Rollback)
                if (oCompany.InTransaction) oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                throw;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oPayment);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oJE);
            }
        }
    }
}