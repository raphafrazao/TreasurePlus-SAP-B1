using System;
using System.Collections.Generic;

namespace CalcularEmpréstimo
{
    public class Parcela
    {
        public int Numero { get; set; }
        public DateTime Vencimento { get; set; }
        public double ValorPMT { get; set; }
        public double Juros { get; set; }
        public double Amortizacao { get; set; }
    }

    public class CalculadoraEmprestimos
    {
        // Cálculo Price (Prestação Fixa com controle rigoroso de arredondamento)
        public static List<Parcela> CalcularPrice(double valorFinanciado, double taxaJurosMensal, int qtdParcelas, DateTime dataInicio)
        {
            List<Parcela> cronograma = new List<Parcela>();
            double saldoDevedor = valorFinanciado;

            // Calcula a PMT padrão e arredonda para 2 casas decimais
            double pmt = valorFinanciado * (taxaJurosMensal / (1 - Math.Pow(1 + taxaJurosMensal, -qtdParcelas)));
            pmt = Math.Round(pmt, 2);

            for (int i = 1; i <= qtdParcelas; i++)
            {
                // Juros do mês arredondados
                double jurosMes = Math.Round(saldoDevedor * taxaJurosMensal, 2);
                double amortizacaoMes;
                double valorPmtAtual = pmt;

                // Se for a última parcela, forçamos o zeramento exato do saldo devedor
                if (i == qtdParcelas)
                {
                    amortizacaoMes = Math.Round(saldoDevedor, 2);
                    valorPmtAtual = Math.Round(amortizacaoMes + jurosMes, 2);
                }
                else
                {
                    amortizacaoMes = Math.Round(pmt - jurosMes, 2);
                }

                // Atualiza o saldo devedor
                saldoDevedor -= amortizacaoMes;

                cronograma.Add(new Parcela
                {
                    Numero = i,
                    Vencimento = dataInicio.AddMonths(i),
                    ValorPMT = valorPmtAtual,
                    Juros = jurosMes,
                    Amortizacao = amortizacaoMes
                });
            }
            return cronograma;
        }

        // Cálculo SAC (Amortização Constante com controle rigoroso de arredondamento)
        public static List<Parcela> CalcularSAC(double valorFinanciado, double taxaJurosMensal, int qtdParcelas, DateTime dataInicio)
        {
            List<Parcela> cronograma = new List<Parcela>();
            double saldoDevedor = valorFinanciado;
            double amortizacaoConstante = Math.Round(valorFinanciado / qtdParcelas, 2);

            for (int i = 1; i <= qtdParcelas; i++)
            {
                double jurosMes = Math.Round(saldoDevedor * taxaJurosMensal, 2);
                double amortizacaoMes = amortizacaoConstante;

                if (i == qtdParcelas)
                {
                    amortizacaoMes = Math.Round(saldoDevedor, 2);
                }

                double pmt = Math.Round(amortizacaoMes + jurosMes, 2);
                saldoDevedor -= amortizacaoMes;

                cronograma.Add(new Parcela
                {
                    Numero = i,
                    Vencimento = dataInicio.AddMonths(i),
                    ValorPMT = pmt,
                    Juros = jurosMes,
                    Amortizacao = amortizacaoMes
                });
            }
            return cronograma;
        }
    }
}