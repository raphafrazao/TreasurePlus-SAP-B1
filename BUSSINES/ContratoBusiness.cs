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

        // Adicione este método na sua classe ContratoBusiness
        public string ObterQueryPendenciasContabeis(string credorPN, string contratoExterno, List<string> parcelasSelecionadas)
        {
            if (parcelasSelecionadas == null || parcelasSelecionadas.Count == 0)
                throw new Exception("Nenhuma parcela selecionada para busca.");

            // Formata os números das parcelas para o SQL (ex: '1','2','3')
            string parcelasFiltroSql = "'" + string.Join("','", parcelasSelecionadas) + "'";

            // A MEGA QUERY CONTÁBIL (Ref1 = Contrato | Ref2 = Parcela | Ref3 = Contrato TreasurePlus)
            string queryContab = $@"
                SELECT 
                    'Y' AS 'Baixar', /* Nasce flegado por padrão */
                    T0.TransId AS 'Nº LCM', 
                    T1.Line_ID AS 'Linha',
                    T1.ShortName AS 'Credor', 
                    T1.Ref1 AS 'Ref. 1 (Contrato)', 
                    T1.Ref2 AS 'Ref. 2 (Parcela)', 
                    T1.Ref3Line AS 'Ref. 3 (Origem)',
                    T1.DueDate AS 'Vencimento', 
                    T1.Credit AS 'Valor Original (Crédito)',
                    T1.BalDueCred AS 'Saldo a Pagar'
                FROM OJDT T0
                INNER JOIN JDT1 T1 ON T0.TransId = T1.TransId
                WHERE T1.ShortName = '{credorPN}'
                  AND T1.Ref1 = '{contratoExterno}'
                  AND T1.Ref2 IN ({parcelasFiltroSql})
                  AND T1.Ref3Line = 'Contrato TreasurePlus'
                  AND T1.BalDueCred > 0 /* Traz apenas o que ainda não foi pago/reconciliado */
                ORDER BY T1.Ref2";

            return queryContab;
        }
    }
}