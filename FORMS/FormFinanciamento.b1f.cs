using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Xml;
using CalcularEmpréstimo;
using TreasurePlus.Business;

namespace TreasurePlus
{
    [Form("TreasurePlus.Form1", "FORMS/FormFinanciamento.b1f")]
    class Form1 : UserFormBase
    {
        public Form1()
        {
        }

        public override void OnInitializeComponent()
        {
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_0").Specific));
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_1").Specific));
            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("codePN").Specific));
            this.EditText0.ChooseFromListAfter += new SAPbouiCOM._IEditTextEvents_ChooseFromListAfterEventHandler(this.EditText0_ChooseFromListAfter);
            this.EditText1 = ((SAPbouiCOM.EditText)(this.GetItem("NomePN").Specific));
            this.EditText1.ChooseFromListAfter += new SAPbouiCOM._IEditTextEvents_ChooseFromListAfterEventHandler(this.EditText1_ChooseFromListAfter);

            // Chama o método que aplica o filtro de fornecedores nas lupas
            this.AplicarCondicoes();

            this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("txtCcBanc").Specific));
            this.EditText2.ChooseFromListAfter += new SAPbouiCOM._IEditTextEvents_ChooseFromListAfterEventHandler(this.EditText2_ChooseFromListAfter);
            this.EditText3 = ((SAPbouiCOM.EditText)(this.GetItem("txtCcCP").Specific));
            this.EditText3.ChooseFromListAfter += new SAPbouiCOM._IEditTextEvents_ChooseFromListAfterEventHandler(this.EditText3_ChooseFromListAfter);
            this.EditText4 = ((SAPbouiCOM.EditText)(this.GetItem("txtCcLP").Specific));
            this.EditText4.ChooseFromListAfter += new SAPbouiCOM._IEditTextEvents_ChooseFromListAfterEventHandler(this.EditText4_ChooseFromListAfter);
            this.EditText5 = ((SAPbouiCOM.EditText)(this.GetItem("DespJur").Specific));
            this.EditText5.ChooseFromListAfter += new SAPbouiCOM._IEditTextEvents_ChooseFromListAfterEventHandler(this.EditText5_ChooseFromListAfter);
            this.EditText6 = ((SAPbouiCOM.EditText)(this.GetItem("DespIOF").Specific));
            this.EditText6.ChooseFromListAfter += new SAPbouiCOM._IEditTextEvents_ChooseFromListAfterEventHandler(this.EditText6_ChooseFromListAfter);
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_8").Specific));
            this.StaticText3 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_10").Specific));
            this.EditText7 = ((SAPbouiCOM.EditText)(this.GetItem("JurosCP").Specific));
            this.EditText7.ChooseFromListAfter += new SAPbouiCOM._IEditTextEvents_ChooseFromListAfterEventHandler(this.EditText7_ChooseFromListAfter);
            this.EditText8 = ((SAPbouiCOM.EditText)(this.GetItem("JurosLP").Specific));
            this.EditText8.ChooseFromListAfter += new SAPbouiCOM._IEditTextEvents_ChooseFromListAfterEventHandler(this.EditText8_ChooseFromListAfter);
            this.EditText9 = ((SAPbouiCOM.EditText)(this.GetItem("ccBanc").Specific));
            this.EditText10 = ((SAPbouiCOM.EditText)(this.GetItem("ccCP").Specific));
            this.EditText11 = ((SAPbouiCOM.EditText)(this.GetItem("CcLP").Specific));
            this.EditText12 = ((SAPbouiCOM.EditText)(this.GetItem("tJurosCP").Specific));
            this.EditText13 = ((SAPbouiCOM.EditText)(this.GetItem("tJurosLP").Specific));
            this.EditText14 = ((SAPbouiCOM.EditText)(this.GetItem("tDespJur").Specific));
            this.EditText15 = ((SAPbouiCOM.EditText)(this.GetItem("tDespIOF").Specific));
            this.EditText16 = ((SAPbouiCOM.EditText)(this.GetItem("VPL").Specific));
            this.EditText17 = ((SAPbouiCOM.EditText)(this.GetItem("txtTaxa").Specific));
            this.EditText18 = ((SAPbouiCOM.EditText)(this.GetItem("txtIOF").Specific));
            this.EditText19 = ((SAPbouiCOM.EditText)(this.GetItem("Parc").Specific));
            this.ComboBox0 = ((SAPbouiCOM.ComboBox)(this.GetItem("CB_METODO").Specific));

            // Botão de Calcular Parcelas
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("bt_Calc").Specific));
            this.Button0.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button0_ClickBefore);

            this.Grid2 = ((SAPbouiCOM.Grid)(this.GetItem("plan_calc").Specific));

            // Botão Principal (Adicionar / Procurar)
            this.Button1 = ((SAPbouiCOM.Button)(this.GetItem("1").Specific));
            this.Button1.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button1_ClickBefore);

            this.EditText20 = ((SAPbouiCOM.EditText)(this.GetItem("txtContr").Specific));
            this.EditText20.ChooseFromListAfter += new SAPbouiCOM._IEditTextEvents_ChooseFromListAfterEventHandler(this.EditText20_ChooseFromListAfter);

            // Botões de Navegação (Setinhas)
            this.btnAnt = ((SAPbouiCOM.Button)(this.GetItem("btnAnt").Specific));
            this.btnAnt.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnAnt_ClickBefore);

            this.btnProx = ((SAPbouiCOM.Button)(this.GetItem("btnProx").Specific));
            this.btnProx.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnProx_ClickBefore);

            this.OnCustomInitialize();
        }
        public override void OnInitializeFormEvents()
        {
        }

        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.StaticText StaticText1;
        private SAPbouiCOM.EditText EditText0;
        private SAPbouiCOM.EditText EditText1;
        private SAPbouiCOM.EditText EditText2;
        private SAPbouiCOM.EditText EditText3;
        private SAPbouiCOM.EditText EditText4;
        private SAPbouiCOM.EditText EditText5;
        private SAPbouiCOM.EditText EditText6;
        private SAPbouiCOM.StaticText StaticText2;
        private SAPbouiCOM.StaticText StaticText3;
        private SAPbouiCOM.EditText EditText7;
        private SAPbouiCOM.EditText EditText8;
        private SAPbouiCOM.EditText EditText9;
        private SAPbouiCOM.EditText EditText10;
        private SAPbouiCOM.EditText EditText11;
        private SAPbouiCOM.EditText EditText12;
        private SAPbouiCOM.EditText EditText13;
        private SAPbouiCOM.EditText EditText14;
        private SAPbouiCOM.EditText EditText15;
        private SAPbouiCOM.Grid Grid2;
        private SAPbouiCOM.EditText EditText16;
        private SAPbouiCOM.EditText EditText17;
        private SAPbouiCOM.EditText EditText18;
        private SAPbouiCOM.EditText EditText19;
        private SAPbouiCOM.ComboBox ComboBox0;
        private SAPbouiCOM.Button Button0;
        private SAPbouiCOM.Button Button1;
        private SAPbouiCOM.EditText EditText20;
        private SAPbouiCOM.Button btnAnt;
        private SAPbouiCOM.Button btnProx;


        private void OnCustomInitialize()
        {
            // Mantém a tela permitindo bloqueios
            this.UIAPIRawForm.SupportedModes = 15;
            this.UIAPIRawForm.AutoManaged = true;

            // Define o modo inicial como Adicionar
            this.UIAPIRawForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE;

            // Aplica as regras de bloqueio que criámos (campos cinzas)
            this.BloquearCamposNoProcurar();

            // -------------------------------------------------------------
            // REGRAS DE VISIBILIDADE DAS SETAS DE NAVEGAÇÃO
            // -------------------------------------------------------------

            // 1. Esconder no Modo Adicionar (1)
            this.UIAPIRawForm.Items.Item("btnAnt").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, 1, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("btnProx").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, 1, SAPbouiCOM.BoModeVisualBehavior.mvb_False);

            // 2. Mostrar no Modo Procurar (2)
            this.UIAPIRawForm.Items.Item("btnAnt").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, 2, SAPbouiCOM.BoModeVisualBehavior.mvb_True);
            this.UIAPIRawForm.Items.Item("btnProx").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, 2, SAPbouiCOM.BoModeVisualBehavior.mvb_True);

            // 3. Mostrar no Modo Visualização (4)
            this.UIAPIRawForm.Items.Item("btnAnt").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_True);
            this.UIAPIRawForm.Items.Item("btnProx").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_True);

            // 4. Mostrar no Modo OK / Atualizar (8) <-- A MÁGICA ESTÁ AQUI!
            this.UIAPIRawForm.Items.Item("btnAnt").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, 8, SAPbouiCOM.BoModeVisualBehavior.mvb_True);
            this.UIAPIRawForm.Items.Item("btnProx").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, 8, SAPbouiCOM.BoModeVisualBehavior.mvb_True);
            // -------------------------------------------------------------
            // REGRAS DE HABILITAÇÃO (MANTER OS BOTÕES ACESOS/CLICÁVEIS)
            // -------------------------------------------------------------

            // Manter acesos no Modo OK/Visualização (2)
            this.UIAPIRawForm.Items.Item("btnAnt").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 2, SAPbouiCOM.BoModeVisualBehavior.mvb_True);
            this.UIAPIRawForm.Items.Item("btnProx").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 2, SAPbouiCOM.BoModeVisualBehavior.mvb_True);

            // Manter acesos no Modo Procurar (4)
            this.UIAPIRawForm.Items.Item("btnAnt").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_True);
            this.UIAPIRawForm.Items.Item("btnProx").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_True);

            // Manter acesos no Modo OK / Atualizar (8)
            this.UIAPIRawForm.Items.Item("btnAnt").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 8, SAPbouiCOM.BoModeVisualBehavior.mvb_True);
            this.UIAPIRawForm.Items.Item("btnProx").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 8, SAPbouiCOM.BoModeVisualBehavior.mvb_True);
            // Força os botões a nascerem invisíveis na abertura da tela (Adicionar)
            this.btnAnt.Item.Visible = false;
            this.btnProx.Item.Visible = false;
        }
        private void BloquearCamposNoProcurar()
        {
            // Bloqueando Valores e Datas
            this.UIAPIRawForm.Items.Item("VPL").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("txtTaxa").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("txtIOF").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("Parc").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("CB_METODO").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);

            // AS DUAS LINHAS ABAIXO FORAM DESCOMENTADAS:
            this.UIAPIRawForm.Items.Item("txtDtIni").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("txtDtFim").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);

            // Bloqueando Nome do Credor
            this.UIAPIRawForm.Items.Item("NomePN").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("codePN").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);

            // Bloqueando Códigos das Contas Contábeis 
            this.UIAPIRawForm.Items.Item("txtCcBanc").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("txtCcCP").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("txtCcLP").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("DespJur").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("DespIOF").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("JurosCP").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("JurosLP").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);

            // Bloqueando Nomes das Contas Contábeis (Descrições)
            this.UIAPIRawForm.Items.Item("ccBanc").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("ccCP").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("CcLP").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("tJurosCP").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("tJurosLP").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("tDespJur").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            this.UIAPIRawForm.Items.Item("tDespIOF").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
        }

        #region MÉTODO DAS LUPAS 

        // --- MÉTODO 1: LUPA DO CÓDIGO ---
        private void EditText0_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {

            SAPbouiCOM.ISBOChooseFromListEventArg oCFLEvento = null;
            oCFLEvento = (SAPbouiCOM.ISBOChooseFromListEventArg)pVal;

            SAPbouiCOM.DataTable oDataTable = null;
            oDataTable = oCFLEvento.SelectedObjects;

            if (oDataTable == null)
                return;

            var codigo = oDataTable.GetValue("CardCode", 0).ToString();
            var nome = oDataTable.GetValue("CardName", 0).ToString();

            this.UIAPIRawForm.DataSources.UserDataSources.Item("UDS_Credor").ValueEx = codigo;
            this.UIAPIRawForm.DataSources.UserDataSources.Item("UDS_Nome").ValueEx = nome;


        }

        // --- MÉTODO 2: LUPA DO NOME ---
        private void EditText1_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            SAPbouiCOM.ISBOChooseFromListEventArg oCFLEvento = null;
            oCFLEvento = (SAPbouiCOM.ISBOChooseFromListEventArg)pVal;

            SAPbouiCOM.DataTable oDataTable = null;
            oDataTable = oCFLEvento.SelectedObjects;

            if (oDataTable == null)
                return;

            var codigo = oDataTable.GetValue("CardCode", 0).ToString();
            var nome = oDataTable.GetValue("CardName", 0).ToString();

            this.UIAPIRawForm.DataSources.UserDataSources.Item("UDS_Credor").ValueEx = codigo;
            this.UIAPIRawForm.DataSources.UserDataSources.Item("UDS_Nome").ValueEx = nome;
        }
        // --- MÉTODO 2: LUPA CONTA BANCÁRIA

        private void EditText2_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            SAPbouiCOM.ISBOChooseFromListEventArg oCFLEvento = null;
            oCFLEvento = (SAPbouiCOM.ISBOChooseFromListEventArg)pVal;

            SAPbouiCOM.DataTable oDataTable = null;
            oDataTable = oCFLEvento.SelectedObjects;

            if (oDataTable == null)
                return;

            var conta = oDataTable.GetValue("AcctCode", 0).ToString();
            var nome = oDataTable.GetValue("AcctName", 0).ToString();


            this.UIAPIRawForm.DataSources.UserDataSources.Item("udDCC").ValueEx = conta;
            this.UIAPIRawForm.DataSources.UserDataSources.Item("uccBanc").ValueEx = nome;

        }

        // -- MÉTODO  LUPA CONTA Financiamento Curto Prazo
        private void EditText3_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            SAPbouiCOM.ISBOChooseFromListEventArg oCFLEvento = null;
            oCFLEvento = (SAPbouiCOM.ISBOChooseFromListEventArg)pVal;

            SAPbouiCOM.DataTable oDataTable = null;
            oDataTable = oCFLEvento.SelectedObjects;

            if (oDataTable == null)
                return;

            var conta = oDataTable.GetValue("AcctCode", 0).ToString();
            var nome = oDataTable.GetValue("AcctName", 0).ToString();


            this.UIAPIRawForm.DataSources.UserDataSources.Item("udCP").ValueEx = conta;
            this.UIAPIRawForm.DataSources.UserDataSources.Item("uCcCP").ValueEx = nome;


        }

        // -- MÉTODO  LUPA CONTA Financiamento Longo Prazo 
        private void EditText4_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            SAPbouiCOM.ISBOChooseFromListEventArg oCFLEvento = null;
            oCFLEvento = (SAPbouiCOM.ISBOChooseFromListEventArg)pVal;

            SAPbouiCOM.DataTable oDataTable = null;
            oDataTable = oCFLEvento.SelectedObjects;

            if (oDataTable == null)
                return;

            var conta = oDataTable.GetValue("AcctCode", 0).ToString();
            var nome = oDataTable.GetValue("AcctName", 0).ToString();


            this.UIAPIRawForm.DataSources.UserDataSources.Item("udLP").ValueEx = conta;
            this.UIAPIRawForm.DataSources.UserDataSources.Item("uccLP").ValueEx = nome;


        }


        // -- MÉTODO  LUPA CONTA (-) Juros a Apropriar Curto Prazo
        private void EditText7_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            SAPbouiCOM.ISBOChooseFromListEventArg oCFLEvento = null;
            oCFLEvento = (SAPbouiCOM.ISBOChooseFromListEventArg)pVal;

            SAPbouiCOM.DataTable oDataTable = null;
            oDataTable = oCFLEvento.SelectedObjects;

            if (oDataTable == null)
                return;

            var conta = oDataTable.GetValue("AcctCode", 0).ToString();
            var nome = oDataTable.GetValue("AcctName", 0).ToString();


            this.UIAPIRawForm.DataSources.UserDataSources.Item("udJCP").ValueEx = conta;
            this.UIAPIRawForm.DataSources.UserDataSources.Item("utJurosCP").ValueEx = nome;


        }

        // -- MÉTODO  LUPA CONTA (-) Juros a Apropriar Longo Prazo 
        private void EditText8_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            SAPbouiCOM.ISBOChooseFromListEventArg oCFLEvento = null;
            oCFLEvento = (SAPbouiCOM.ISBOChooseFromListEventArg)pVal;

            SAPbouiCOM.DataTable oDataTable = null;
            oDataTable = oCFLEvento.SelectedObjects;

            if (oDataTable == null)
                return;

            var conta = oDataTable.GetValue("AcctCode", 0).ToString();
            var nome = oDataTable.GetValue("AcctName", 0).ToString();


            this.UIAPIRawForm.DataSources.UserDataSources.Item("udJLP").ValueEx = conta;
            this.UIAPIRawForm.DataSources.UserDataSources.Item("utJurosLP").ValueEx = nome;

        }


        // -- MÉTODO  LUPA CONTA Despesas com Juros 

        private void EditText5_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            SAPbouiCOM.ISBOChooseFromListEventArg oCFLEvento = null;
            oCFLEvento = (SAPbouiCOM.ISBOChooseFromListEventArg)pVal;

            SAPbouiCOM.DataTable oDataTable = null;
            oDataTable = oCFLEvento.SelectedObjects;

            if (oDataTable == null)
                return;

            var conta = oDataTable.GetValue("AcctCode", 0).ToString();
            var nome = oDataTable.GetValue("AcctName", 0).ToString();


            this.UIAPIRawForm.DataSources.UserDataSources.Item("udDJ").ValueEx = conta;
            this.UIAPIRawForm.DataSources.UserDataSources.Item("utDespJur").ValueEx = nome;

        }

        // -- MÉTODO  LUPA CONTA Despesas com IOF
        private void EditText6_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            SAPbouiCOM.ISBOChooseFromListEventArg oCFLEvento = null;
            oCFLEvento = (SAPbouiCOM.ISBOChooseFromListEventArg)pVal;

            SAPbouiCOM.DataTable oDataTable = null;
            oDataTable = oCFLEvento.SelectedObjects;

            if (oDataTable == null)
                return;

            var conta = oDataTable.GetValue("AcctCode", 0).ToString();
            var nome = oDataTable.GetValue("AcctName", 0).ToString();


            this.UIAPIRawForm.DataSources.UserDataSources.Item("udDIOF").ValueEx = conta;
            this.UIAPIRawForm.DataSources.UserDataSources.Item("utDespIOF").ValueEx = nome;

        }
        private void EditText20_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                SAPbouiCOM.ISBOChooseFromListEventArg cflEventArgs = (SAPbouiCOM.ISBOChooseFromListEventArg)pVal;

                if (cflEventArgs.SelectedObjects != null && !cflEventArgs.SelectedObjects.IsEmpty)
                {
                    // A lupa abriu baseada no DocEntry, mas aqui resgatamos a coluna do número externo!
                    string contrato = cflEventArgs.SelectedObjects.GetValue("U_NumContrato", 0).ToString();
                    ((SAPbouiCOM.EditText)this.GetItem("txtContr").Specific).Value = contrato;
                }
            }
            catch { }
        }


        private void AplicarCondicoes()
        {
            try
            {
                SAPbouiCOM.Conditions oCons = null;
                SAPbouiCOM.Condition oCon = null;

                // ========================================================
                // 1. FILTRO DE FORNECEDORES (Parceiros de Negócios)
                // ========================================================
                oCons = this.UIAPIRawForm.ChooseFromLists.Item("cfCodPN").GetConditions();
                oCon = oCons.Add();
                oCon.Alias = "CardType";
                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oCon.CondVal = "S"; // Apenas Fornecedores

                this.UIAPIRawForm.ChooseFromLists.Item("cfCodPN").SetConditions(oCons);
                this.UIAPIRawForm.ChooseFromLists.Item("cfNomPN").SetConditions(oCons);


                // ========================================================
                // 2. FILTRO DE CONTAS CONTÁBEIS (Apenas contas que aceitam lançamento)
                // ========================================================
                // Criamos novas variáveis de condição para não misturar com a de cima
                SAPbouiCOM.Conditions oConsConta = this.UIAPIRawForm.ChooseFromLists.Item("CFL_CC").GetConditions();
                SAPbouiCOM.Condition oConConta = oConsConta.Add();

                oConConta.Alias = "Postable";
                oConConta.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oConConta.CondVal = "Y"; // "Y" (Yes) garante que o usuário não selecione uma Conta Título

                // Aplica a regra no seu CFL de Contas
                this.UIAPIRawForm.ChooseFromLists.Item("CFL_CC").SetConditions(oConsConta);
                this.UIAPIRawForm.ChooseFromLists.Item("CFL_CP").SetConditions(oConsConta);
                this.UIAPIRawForm.ChooseFromLists.Item("CFL_LP").SetConditions(oConsConta);
                this.UIAPIRawForm.ChooseFromLists.Item("CFL_DJ").SetConditions(oConsConta);
                this.UIAPIRawForm.ChooseFromLists.Item("CFL_IOF").SetConditions(oConsConta);
                this.UIAPIRawForm.ChooseFromLists.Item("CFL_JLP").SetConditions(oConsConta);
                this.UIAPIRawForm.ChooseFromLists.Item("CFL_JCP").SetConditions(oConsConta);
                this.UIAPIRawForm.ChooseFromLists.Item("CFL_CC").SetConditions(oConsConta);

            }
            catch (Exception ex)
            {
                SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Erro ao aplicar filtros: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }

        #endregion



        private void Button0_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)// calcular as parcelas 
        {
            BubbleEvent = true;
            try
            {
                SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Calculando parcelas, por favor aguarde...", SAPbouiCOM.BoMessageTime.bmt_Medium, false);
                this.UIAPIRawForm.Freeze(true);

                double vpl = Convert.ToDouble(((SAPbouiCOM.EditText)this.GetItem("VPL").Specific).Value.Replace(".", ","));
                DateTime dt = DateTime.ParseExact(((SAPbouiCOM.EditText)this.GetItem("txtDtIni").Specific).Value, "yyyyMMdd", null);
                double taxa = Convert.ToDouble(((SAPbouiCOM.EditText)this.GetItem("txtTaxa").Specific).Value.Replace(".", ",")) / 100.0;
                int parc = Convert.ToInt32(((SAPbouiCOM.EditText)this.GetItem("Parc").Specific).Value);
                string metodo = ((SAPbouiCOM.ComboBox)this.GetItem("CB_METODO").Specific).Value;

                List<Parcela> cronograma;
                if (metodo == "2") // SAC
                    cronograma = CalculadoraEmprestimos.CalcularSAC(vpl, taxa, parc, dt);
                else // Price
                    cronograma = CalculadoraEmprestimos.CalcularPrice(vpl, taxa, parc, dt);

                if (cronograma != null && cronograma.Count > 0)
                {
                    double somaAmortizacao = 0;
                    foreach (var p in cronograma) somaAmortizacao += p.Amortizacao;

                    double diferenca = Math.Round(vpl - somaAmortizacao, 2);
                    if (diferenca != 0)
                    {
                        var ultimaParcela = cronograma[cronograma.Count - 1];
                        ultimaParcela.Amortizacao += diferenca;
                        ultimaParcela.ValorPMT = ultimaParcela.Amortizacao + ultimaParcela.Juros;
                    }
                }

                SAPbouiCOM.DataTable oDataTable = this.UIAPIRawForm.DataSources.DataTables.Item("DT_PROJ");
                oDataTable.Rows.Clear();

                foreach (var p in cronograma)
                {
                    oDataTable.Rows.Add();
                    int i = oDataTable.Rows.Count - 1;
                    oDataTable.SetValue("Parc", i, p.Numero);
                    oDataTable.SetValue("Venc", i, p.Vencimento);
                    oDataTable.SetValue("VlParc", i, p.ValorPMT);
                    oDataTable.SetValue("Juros", i, p.Juros);
                    oDataTable.SetValue("Amort", i, p.Amortizacao);
                }

                SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Cálculo efetuado com sucesso!", SAPbouiCOM.BoMessageTime.bmt_Short, false);
            }
            catch (Exception ex)
            {
                SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Erro: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }


        }



        private void Button1_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            {
                BubbleEvent = true;

                #region MODO ADICIONAR DA TELA 
                if (this.UIAPIRawForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                {
                    // VERIFICA E PREENCHE A COMMONCLASS SE ELA ESTIVER VAZIA:
                    if (TreasurePlus.CORE.CommomClass.oCompany == null)
                    {
                        TreasurePlus.CORE.CommomClass.oCompany = (SAPbobsCOM.Company)SAPbouiCOM.Framework.Application.SBO_Application.Company.GetDICompany();
                    }

                    SAPbobsCOM.Company oCompany = TreasurePlus.CORE.CommomClass.oCompany;

                    try
                    {
                        SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Processando contrato e gerando Lançamento Contábil. Por favor, aguarde...", SAPbouiCOM.BoMessageTime.bmt_Medium, false);
                        if (!oCompany.InTransaction) oCompany.StartTransaction();

                        // Ajustado para os nomes exatos do seu XML!
                        string contratoId = ((SAPbouiCOM.EditText)this.GetItem("txtContr").Specific).Value.Trim();
                        string codePN = ((SAPbouiCOM.EditText)this.GetItem("codePN").Specific).Value.Trim();
                        double vpl = Convert.ToDouble(((SAPbouiCOM.EditText)this.GetItem("VPL").Specific).Value.Replace(".", ","));
                        double valorIof = Convert.ToDouble(((SAPbouiCOM.EditText)this.GetItem("txtIOF").Specific).Value.Replace(".", ","));
                        DateTime dtIni = DateTime.ParseExact(((SAPbouiCOM.EditText)this.GetItem("txtDtIni").Specific).Value, "yyyyMMdd", null);
                        DateTime dtFim = DateTime.ParseExact(((SAPbouiCOM.EditText)this.GetItem("txtDtFim").Specific).Value, "yyyyMMdd", null);

                        double taxa = Convert.ToDouble(((SAPbouiCOM.EditText)this.GetItem("txtTaxa").Specific).Value.Replace(".", ","));
                        int parcelas = Convert.ToInt32(((SAPbouiCOM.EditText)this.GetItem("Parc").Specific).Value);
                        string metodo = ((SAPbouiCOM.ComboBox)this.GetItem("CB_METODO").Specific).Value;

                        string contaDespJuros = this.UIAPIRawForm.DataSources.UserDataSources.Item("udDJ").Value;
                        string contaBancaria = this.UIAPIRawForm.DataSources.UserDataSources.Item("udDCC").Value;
                        string contaDespIof = this.UIAPIRawForm.DataSources.UserDataSources.Item("udDIOF").Value;
                        string contaCp = this.UIAPIRawForm.DataSources.UserDataSources.Item("udCP").Value;
                        string contaLp = this.UIAPIRawForm.DataSources.UserDataSources.Item("udLP").Value;
                        string contaJurosCp = this.UIAPIRawForm.DataSources.UserDataSources.Item("udJCP").Value;
                        string contaJurosLp = this.UIAPIRawForm.DataSources.UserDataSources.Item("udJLP").Value;

                        SAPbouiCOM.DataTable oGridData = this.UIAPIRawForm.DataSources.DataTables.Item("DT_PROJ");
                        if (oGridData.Rows.Count == 0) throw new Exception("Por favor, clique em 'Calcular' para gerar as parcelas ou informe antes de adicionar o contrato.");

                        // 1.Instanciamos a sua classe de negócios
                        ContratoBusiness negocioContrato = new ContratoBusiness();

                        // 2. Chamamos a geração do LCM
                        int transIdContabil = negocioContrato.GerarLancamentoContabilContrato(
                            contratoId, codePN, vpl, valorIof, dtIni, dtFim,
                            oGridData, contaBancaria, contaDespIof, contaCp, contaLp, contaJurosCp, contaJurosLp
                        );

                        // 3. Chamamos o salvamento do UDO
                        negocioContrato.SalvarContratoNoUDO(
                            contratoId, codePN, vpl, valorIof, taxa, parcelas, metodo, dtIni, dtFim,
                            oGridData, transIdContabil, contaBancaria, contaCp, contaLp,
                            contaJurosCp, contaJurosLp, contaDespJuros, contaDespIof
                        );

                        // -------------------------------------------------------------

                        if (oCompany.InTransaction) oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);

                        this.UIAPIRawForm.Freeze(true);
                        for (int i = 0; i < oGridData.Rows.Count; i++)
                        {
                            oGridData.SetValue("Status", i, "A");
                            oGridData.SetValue("LCM", i, transIdContabil);
                        }

                        SAPbouiCOM.Grid oGridTela = (SAPbouiCOM.Grid)this.GetItem("plan_calc").Specific;
                        oGridTela.Columns.Item("Status").Editable = false;
                        oGridTela.Columns.Item("LCM").Editable = false;
                        this.UIAPIRawForm.Freeze(false);

                        BubbleEvent = false;
                        SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Contrato e Lançamento Contábil (TransId: " + transIdContabil + ") gerados com sucesso!", SAPbouiCOM.BoMessageTime.bmt_Short, false);

                        this.UIAPIRawForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                    }
                    catch (Exception ex)
                    {
                        if (oCompany.InTransaction) oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        BubbleEvent = false;
                        this.UIAPIRawForm.Freeze(false);
                        SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Erro ao salvar contrato (Operação cancelada): " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, true);
                    }
                }
                #endregion

                #region    MODO PROCURAR DA TELA 

                else if (this.UIAPIRawForm.Mode == SAPbouiCOM.BoFormMode.fm_FIND_MODE)
                {
                    BubbleEvent = true;
                    try
                    {
                        string contratoId = ((SAPbouiCOM.EditText)this.GetItem("txtContr").Specific).Value.Trim();

                        if (string.IsNullOrEmpty(contratoId))
                        {
                            throw new Exception("Digite o número do contrato antes de clicar em Procurar.");
                        }

                        SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Pronto!", SAPbouiCOM.BoMessageTime.bmt_Short, false);

                        this.UIAPIRawForm.Freeze(true);

                        // VERIFICA E PREENCHE A COMMONCLASS SE ELA ESTIVER VAZIA:
                        if (TreasurePlus.CORE.CommomClass.oCompany == null)
                        {
                            TreasurePlus.CORE.CommomClass.oCompany = (SAPbobsCOM.Company)SAPbouiCOM.Framework.Application.SBO_Application.Company.GetDICompany();
                        }

                        SAPbobsCOM.Company oCompany = TreasurePlus.CORE.CommomClass.oCompany;

                        SAPbobsCOM.Recordset oRec = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        oRec.DoQuery($"SELECT * FROM [@TP_LOAN] WHERE CAST(U_NumContrato AS NVARCHAR(MAX)) = '{contratoId}'");

                        if (oRec.RecordCount == 0)
                        {
                            throw new Exception("Contrato não encontrado na base de dados.");
                        }

                        // --- PARCEIRO DE NEGÓCIOS ---
                        string credorCode = oRec.Fields.Item("U_CreditorNumber").Value.ToString();
                        this.UIAPIRawForm.DataSources.UserDataSources.Item("UDS_Credor").Value = credorCode;

                        SAPbobsCOM.Recordset recPN = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        recPN.DoQuery($"SELECT CardName FROM OCRD WHERE CardCode = '{credorCode}'");
                        if (recPN.RecordCount > 0)
                        {
                            this.UIAPIRawForm.DataSources.UserDataSources.Item("UDS_Nome").Value = recPN.Fields.Item("CardName").Value.ToString();
                        }

                        // --- CAMPOS DE VALORES E COMBOBOX ---
                        ((SAPbouiCOM.EditText)this.GetItem("VPL").Specific).Value = oRec.Fields.Item("U_FinancedAmount").Value.ToString();
                        ((SAPbouiCOM.EditText)this.GetItem("txtTaxa").Specific).Value = oRec.Fields.Item("U_Rate").Value.ToString();
                        ((SAPbouiCOM.EditText)this.GetItem("txtIOF").Specific).Value = oRec.Fields.Item("U_IOFValue").Value.ToString();
                        ((SAPbouiCOM.EditText)this.GetItem("Parc").Specific).Value = oRec.Fields.Item("U_Install").Value.ToString();

                        try { ((SAPbouiCOM.ComboBox)this.GetItem("CB_METODO").Specific).Select(oRec.Fields.Item("U_AmortMet").Value.ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue); } catch { }

                        // --- DATAS DO CONTRATO ---
                        DateTime dtIni = Convert.ToDateTime(
                            oRec.Fields.Item("U_StartDate").Value
                        );

                        DateTime dtFim = Convert.ToDateTime(
                            oRec.Fields.Item("U_EndDate").Value
                        );

                        string dataIniSap = dtIni.ToString("yyyyMMdd");
                        string dataFimSap = dtFim.ToString("yyyyMMdd");

                        // UserDataSources corretos conforme o XML.
                        this.UIAPIRawForm.DataSources.UserDataSources
                            .Item("udtIni").ValueEx = dataIniSap;

                        this.UIAPIRawForm.DataSources.UserDataSources
                            .Item("udtfim").ValueEx = dataFimSap;

                        // Campos vinculados aos UDS.
                        SAPbouiCOM.EditText campoDataIni =
                            (SAPbouiCOM.EditText)this.GetItem("txtDtIni").Specific;

                        SAPbouiCOM.EditText campoDataFim =
                            (SAPbouiCOM.EditText)this.GetItem("txtDtFim").Specific;

                        // --- CONTAS CONTÁBEIS (Puxando os Códigos) ---
                        string cBanc = oRec.Fields.Item("U_BankAcc").Value.ToString();
                        string cCP = oRec.Fields.Item("U_ShortTAcc").Value.ToString();
                        string cLP = oRec.Fields.Item("U_LongTAcc").Value.ToString(); // <-- Longo Prazo resgatado!
                        string cJCP = oRec.Fields.Item("U_SIntAcc").Value.ToString();
                        string cJLP = oRec.Fields.Item("U_LIntAcc").Value.ToString();
                        string cDJ = oRec.Fields.Item("U_IntExpAcc").Value.ToString();
                        string cDIOF = oRec.Fields.Item("U_IOFExpAcc").Value.ToString();

                        this.UIAPIRawForm.DataSources.UserDataSources.Item("udDCC").ValueEx = cBanc;
                        this.UIAPIRawForm.DataSources.UserDataSources.Item("udCP").ValueEx = cCP;
                        this.UIAPIRawForm.DataSources.UserDataSources.Item("udLP").ValueEx = cLP;
                        this.UIAPIRawForm.DataSources.UserDataSources.Item("udJCP").ValueEx = cJCP;
                        this.UIAPIRawForm.DataSources.UserDataSources.Item("udJLP").ValueEx = cJLP;
                        this.UIAPIRawForm.DataSources.UserDataSources.Item("udDJ").ValueEx = cDJ;
                        this.UIAPIRawForm.DataSources.UserDataSources.Item("udDIOF").ValueEx = cDIOF;

                        // --- NOMES DAS CONTAS CONTÁBEIS (A Mágica da Descrição) ---
                        SAPbobsCOM.Recordset recConta = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                        if (!string.IsNullOrEmpty(cBanc)) { recConta.DoQuery($"SELECT AcctName FROM OACT WHERE AcctCode = '{cBanc}'"); this.UIAPIRawForm.DataSources.UserDataSources.Item("uccBanc").ValueEx = recConta.Fields.Item(0).Value.ToString(); }
                        if (!string.IsNullOrEmpty(cCP)) { recConta.DoQuery($"SELECT AcctName FROM OACT WHERE AcctCode = '{cCP}'"); this.UIAPIRawForm.DataSources.UserDataSources.Item("uCcCP").ValueEx = recConta.Fields.Item(0).Value.ToString(); }
                        if (!string.IsNullOrEmpty(cLP)) { recConta.DoQuery($"SELECT AcctName FROM OACT WHERE AcctCode = '{cLP}'"); this.UIAPIRawForm.DataSources.UserDataSources.Item("uccLP").ValueEx = recConta.Fields.Item(0).Value.ToString(); }
                        if (!string.IsNullOrEmpty(cJCP)) { recConta.DoQuery($"SELECT AcctName FROM OACT WHERE AcctCode = '{cJCP}'"); this.UIAPIRawForm.DataSources.UserDataSources.Item("utJurosCP").ValueEx = recConta.Fields.Item(0).Value.ToString(); }
                        if (!string.IsNullOrEmpty(cJLP)) { recConta.DoQuery($"SELECT AcctName FROM OACT WHERE AcctCode = '{cJLP}'"); this.UIAPIRawForm.DataSources.UserDataSources.Item("utJurosLP").ValueEx = recConta.Fields.Item(0).Value.ToString(); }
                        if (!string.IsNullOrEmpty(cDJ)) { recConta.DoQuery($"SELECT AcctName FROM OACT WHERE AcctCode = '{cDJ}'"); this.UIAPIRawForm.DataSources.UserDataSources.Item("utDespJur").ValueEx = recConta.Fields.Item(0).Value.ToString(); }
                        if (!string.IsNullOrEmpty(cDIOF)) { recConta.DoQuery($"SELECT AcctName FROM OACT WHERE AcctCode = '{cDIOF}'"); this.UIAPIRawForm.DataSources.UserDataSources.Item("utDespIOF").ValueEx = recConta.Fields.Item(0).Value.ToString(); }

                        string docEntry = oRec.Fields.Item("DocEntry").Value.ToString();
                        SAPbouiCOM.DataTable oDataTable = this.UIAPIRawForm.DataSources.DataTables.Item("DT_PROJ");

                        string queryGrid = $"SELECT U_InstNum AS 'Parcela', U_DueDate AS 'Vencimento', U_InstAmt AS 'Valor da Parcela', U_Interest AS 'Juros', U_Amort AS 'Amortização', U_Status AS 'Status', U_JE_Aprop AS 'LCM' FROM [@TP_LOAN_LINES] WHERE DocEntry = {docEntry} ORDER BY U_InstNum";
                        oDataTable.ExecuteQuery(queryGrid);
                        // Pega o Grid da tela
                        SAPbouiCOM.Grid oGrid = (SAPbouiCOM.Grid)this.GetItem("plan_calc").Specific;

                        // Transforma a coluna "LCM" numa coluna do tipo Link
                        SAPbouiCOM.EditTextColumn colLCM = (SAPbouiCOM.EditTextColumn)oGrid.Columns.Item("LCM");
                        colLCM.LinkedObjectType = "30"; // 30 = Lançamento Contábil (LCM)

                        // 1. TIRA O CURSOR DA DATA E MANDA PARA O BOTÃO OK (Evita o erro 66000-23)
                        this.UIAPIRawForm.ActiveItem = "txtContr";
                        // Trava explicitamente as Datas, o Contrato e o Credor para ficarem cinzentos
                        
                        this.UIAPIRawForm.Mode = SAPbouiCOM.BoFormMode.fm_VIEW_MODE;
                    }
                    catch (Exception ex)
                    {
                        SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Erro ao buscar: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, true);
                        BubbleEvent = false;
                    }
                    finally
                    {
                        this.UIAPIRawForm.Freeze(false);
                    }
                }
                #endregion 

            }
        }



        private void NavegarContrato(string direcao)
        {
            try
            {
                if (this.UIAPIRawForm.Mode != SAPbouiCOM.BoFormMode.fm_VIEW_MODE &&
                    this.UIAPIRawForm.Mode != SAPbouiCOM.BoFormMode.fm_FIND_MODE) return;

                string contratoAtual = ((SAPbouiCOM.EditText)this.GetItem("txtContr").Specific).Value.Trim();

                SAPbobsCOM.Company oCompany = TreasurePlus.CORE.CommomClass.oCompany ?? (SAPbobsCOM.Company)SAPbouiCOM.Framework.Application.SBO_Application.Company.GetDICompany();
                SAPbobsCOM.Recordset oRec = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                string query = "";
                // Truque do CAST para driblar a limitação do tipo ntext no SQL Server
                string campoNvarchar = "CAST(U_NumContrato AS NVARCHAR(250))";

                if (string.IsNullOrEmpty(contratoAtual))
                {
                    if (direcao == "PROXIMO")
                        query = $"SELECT TOP 1 {campoNvarchar} FROM [@TP_LOAN] ORDER BY {campoNvarchar} ASC";
                    else if (direcao == "ANTERIOR")
                        query = $"SELECT TOP 1 {campoNvarchar} FROM [@TP_LOAN] ORDER BY {campoNvarchar} DESC";
                }
                else
                {
                    if (direcao == "PROXIMO")
                        query = $"SELECT TOP 1 {campoNvarchar} FROM [@TP_LOAN] WHERE {campoNvarchar} > '{contratoAtual}' ORDER BY {campoNvarchar} ASC";
                    else if (direcao == "ANTERIOR")
                        query = $"SELECT TOP 1 {campoNvarchar} FROM [@TP_LOAN] WHERE {campoNvarchar} < '{contratoAtual}' ORDER BY {campoNvarchar} DESC";
                }

                oRec.DoQuery(query);

                if (oRec.RecordCount > 0)
                {
                    string novoContrato = oRec.Fields.Item(0).Value.ToString();

                    if (this.UIAPIRawForm.Mode != SAPbouiCOM.BoFormMode.fm_FIND_MODE)
                    {
                        this.UIAPIRawForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE;
                    }

                    // Injeta o número na tela
                    ((SAPbouiCOM.EditText)this.GetItem("txtContr").Specific).Value = novoContrato;

                    // Chama o seu método de busca maravilhosamente construído de forma direta!
                    bool bubble = true;
                    this.Button1_ClickBefore(null, null, out bubble);
                }
                else
                {
                    SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Não há contratos registados nesta direção.", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                }
            }
            catch (Exception ex)
            {
                SAPbouiCOM.Framework.Application.SBO_Application.SetStatusBarMessage("Erro na navegação: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }

        private void btnAnt_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            NavegarContrato("ANTERIOR");
        }

        private void btnProx_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            NavegarContrato("PROXIMO");
        }
    }

}