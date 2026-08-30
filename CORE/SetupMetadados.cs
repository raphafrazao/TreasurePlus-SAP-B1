using System;
using SAPbobsCOM;

namespace TreasurePlus
{
    public class SetupMetadados
    {
        private SAPbobsCOM.Company _oCompany;

        
        public SetupMetadados(SAPbobsCOM.Company company)
        {
            _oCompany = company;
        }

        public void InstalarEstruturaTreasurePlus()
        {
            try
            {
                // 1. CRIAR AS TABELAS (UDTs)
                CriarTabela("TP_LOAN", "Contratos de Empréstimos", BoUTBTableType.bott_Document);
                CriarTabela("TP_LOAN_LINES", "Contratos de Empréstimos Linha", BoUTBTableType.bott_DocumentLines);

                // 2. CRIAR OS CAMPOS DE CABEÇALHO (@TP_LOAN)
                CriarCampo("TP_LOAN", "CreditorNumber", "Parceiro de Negócios", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 15);
                CriarCampo("TP_LOAN", "StartDate", "Data de Início", BoFieldTypes.db_Date, BoFldSubTypes.st_None, 10);
                CriarCampo("TP_LOAN", "EndDate", "Data Fim", BoFieldTypes.db_Date, BoFldSubTypes.st_None, 10);
                CriarCampo("TP_LOAN", "FinancedAmount", "Valor Financiado", BoFieldTypes.db_Float, BoFldSubTypes.st_Price, 11);
                CriarCampo("TP_LOAN", "IOFValue", "Valor IOF", BoFieldTypes.db_Float, BoFldSubTypes.st_Price, 11);
                CriarCampo("TP_LOAN", "Rate", "Taxa de Juros", BoFieldTypes.db_Float, BoFldSubTypes.st_Rate, 11);
                CriarCampo("TP_LOAN", "Install", "Parcelas", BoFieldTypes.db_Numeric, BoFldSubTypes.st_None, 10);
                CriarCampo("TP_LOAN", "AmortMet", "Método Amortização", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 10);
                CriarCampo("TP_LOAN", "BankAcc", "Conta Bancária", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 15);
                CriarCampo("TP_LOAN", "ShortTAcc", "Conta C. Contábil CP", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 15);
                CriarCampo("TP_LOAN", "LIntAcc", "(-) Juros Apropriar LP", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 15);
                CriarCampo("TP_LOAN", "SIntAcc", "(-) Juros Apropriar CP", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 15);
                CriarCampo("TP_LOAN", "IntExpAcc", "Despesas Juros", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 15);
                CriarCampo("TP_LOAN", "IOFExpAcc", "Despesa IOF", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 15);
                CriarCampo("TP_LOAN", "LongTAcc", "Conta C. Contábil LP", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 15);
                CriarCampo("TP_LOAN", "NumContrato", "Numero Contrato Externo", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 50);

                CriarCampo("TP_LOAN", "BPLId", "Filial do Contrato", BoFieldTypes.db_Numeric, BoFldSubTypes.st_None, 11);// versão 1.0.0.1



                // NOTA: Ajustado para Alpha 10 para suportar valores como "Aberto" / "A", em vez de estritamente numérico.
                CriarCampo("TP_LOAN", "Status", "Status", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 10);

                // 3. CRIAR OS CAMPOS DAS LINHAS (@TP_LOAN_LINES)
                CriarCampo("TP_LOAN_LINES", "InstNum", "Nº da Parcela", BoFieldTypes.db_Numeric, BoFldSubTypes.st_None, 11);
                CriarCampo("TP_LOAN_LINES", "DueDate", "Data de Vencimento", BoFieldTypes.db_Date, BoFldSubTypes.st_None, 10);
                CriarCampo("TP_LOAN_LINES", "InstAmt", "Valor da Parcela", BoFieldTypes.db_Float, BoFldSubTypes.st_Price, 11);
                CriarCampo("TP_LOAN_LINES", "Interest", "Juros da Parcela", BoFieldTypes.db_Float, BoFldSubTypes.st_Price, 11);
                CriarCampo("TP_LOAN_LINES", "Amort", "Amortização", BoFieldTypes.db_Float, BoFldSubTypes.st_Price, 11);
                CriarCampo("TP_LOAN_LINES", "Status", "Status da Parcela", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 10);

                // Campos Vinculados a Objetos do Sistema!
                CriarCampo("TP_LOAN_LINES", "JE_Aprop", "LCM Apropriação", BoFieldTypes.db_Numeric, BoFldSubTypes.st_None, 11, UDFLinkedSystemObjectTypesEnum.ulJournalEntries);
                CriarCampo("TP_LOAN_LINES", "PayDoc", "Nº Pagamento Efetuado", BoFieldTypes.db_Numeric, BoFldSubTypes.st_None, 11, UDFLinkedSystemObjectTypesEnum.ulVendorPayments);

                // 4. REGISTRAR O UDO (User Defined Object)
                RegistrarUDO("TP_LOAN", "TreasurePlus - Empréstimos", "TP_LOAN", "TP_LOAN_LINES");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao instalar metadados TreasurePlus: " + ex.Message);
            }
        }

        private void CriarTabela(string nomeTabela, string descricao, BoUTBTableType tipoTabela)
        {
            SAPbobsCOM.UserTablesMD oUserTablesMD = null;
            try
            {
                oUserTablesMD = (SAPbobsCOM.UserTablesMD)_oCompany.GetBusinessObject(BoObjectTypes.oUserTables);
                if (!oUserTablesMD.GetByKey(nomeTabela))
                {
                    oUserTablesMD.TableName = nomeTabela;
                    oUserTablesMD.TableDescription = descricao;
                    oUserTablesMD.TableType = tipoTabela;

                    int lRetCode = oUserTablesMD.Add();
                    if (lRetCode != 0)
                        throw new Exception(_oCompany.GetLastErrorDescription());
                }
            }
            finally
            {
                if (oUserTablesMD != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserTablesMD);
            }
        }

        private void CriarCampo(string nomeTabela, string nomeCampo, string descricao, BoFieldTypes tipo, BoFldSubTypes subTipo, int tamanho, UDFLinkedSystemObjectTypesEnum? linkObjeto = null)
        {
            SAPbobsCOM.UserFieldsMD oUserFieldsMD = null;
            try
            {
                oUserFieldsMD = (SAPbobsCOM.UserFieldsMD)_oCompany.GetBusinessObject(BoObjectTypes.oUserFields);

                // Valida se o campo já existe para não dar erro
                int tableId = GetFieldID(nomeTabela, nomeCampo);
                if (tableId == -1)
                {
                    oUserFieldsMD.TableName = nomeTabela;
                    oUserFieldsMD.Name = nomeCampo;
                    oUserFieldsMD.Description = descricao;
                    oUserFieldsMD.Type = tipo;
                    oUserFieldsMD.SubType = subTipo;

                    if (tipo != BoFieldTypes.db_Date)
                        oUserFieldsMD.EditSize = tamanho;

                    // Aplica o link nativo do SAP (Lançamento Contábil / Pagamentos)
                    if (linkObjeto.HasValue)
                        oUserFieldsMD.LinkedSystemObject = linkObjeto.Value;

                    int lRetCode = oUserFieldsMD.Add();
                    if (lRetCode != 0)
                        throw new Exception(_oCompany.GetLastErrorDescription());
                }
            }
            finally
            {
                if (oUserFieldsMD != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserFieldsMD);
            }
        }

        private void RegistrarUDO(string codigoUDO, string nomeUDO, string tabelaPai, string tabelaFilha)
        {
            SAPbobsCOM.UserObjectsMD oUserObjectMD = null;
            try
            {
                oUserObjectMD = (SAPbobsCOM.UserObjectsMD)_oCompany.GetBusinessObject(BoObjectTypes.oUserObjectsMD);
                if (!oUserObjectMD.GetByKey(codigoUDO))
                {
                    oUserObjectMD.Code = codigoUDO;
                    oUserObjectMD.Name = nomeUDO;
                    oUserObjectMD.ObjectType = BoUDOObjType.boud_Document;
                    oUserObjectMD.TableName = tabelaPai;

                    oUserObjectMD.CanCancel = BoYesNoEnum.tYES;
                    oUserObjectMD.CanClose = BoYesNoEnum.tYES;
                    oUserObjectMD.CanDelete = BoYesNoEnum.tYES;
                    oUserObjectMD.CanFind = BoYesNoEnum.tYES;
                    oUserObjectMD.CanLog = BoYesNoEnum.tYES;

                    // Configuração da Tabela Filha
                    oUserObjectMD.ChildTables.TableName = tabelaFilha;
                    oUserObjectMD.ChildTables.Add();

                    // Define os campos que aparecerão no Modo de Busca (Binóculos)
                    oUserObjectMD.FindColumns.ColumnAlias = "DocEntry";
                    oUserObjectMD.FindColumns.ColumnDescription = "Nº Interno";
                    oUserObjectMD.FindColumns.Add();

                    oUserObjectMD.FindColumns.ColumnAlias = "U_NumContrato";
                    oUserObjectMD.FindColumns.ColumnDescription = "Contrato";
                    oUserObjectMD.FindColumns.Add();

                    int lRetCode = oUserObjectMD.Add();
                    if (lRetCode != 0)
                        throw new Exception(_oCompany.GetLastErrorDescription());
                }
            }
            finally
            {
                if (oUserObjectMD != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjectMD);
            }
        }

        private int GetFieldID(string tableName, string fieldName)
        {
            SAPbobsCOM.Recordset oRec = (SAPbobsCOM.Recordset)_oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                string sql = "";

                if (_oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    // HANA: Aspas duplas na tabela padrão CUFD e nas colunas
                    sql = $"SELECT \"FieldID\" FROM \"CUFD\" WHERE \"TableID\" = '@{tableName}' AND \"AliasID\" = '{fieldName}'";
                }
                else
                {
                    // SQL SERVER: Escrita normal
                    sql = $"SELECT FieldID FROM CUFD WHERE TableID = '@{tableName}' AND AliasID = '{fieldName}'";
                }

                oRec.DoQuery(sql);
                if (oRec.RecordCount > 0)
                    return Convert.ToInt32(oRec.Fields.Item(0).Value);
                return -1;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oRec);
            }
        }
    }
}
