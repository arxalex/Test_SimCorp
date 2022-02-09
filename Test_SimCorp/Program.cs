namespace Test_SimCorp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            ICalculation calculation = new Calculation();
            Console.WriteLine("Start default or custom (1/2)");
            var key = Console.ReadKey();
            Console.WriteLine("\n");

            string agreementDate, calculationDate, x, r, n, method;

            switch (key.Key)
            {
                case ConsoleKey.D1:
                    agreementDate = "2020-01-01";
                    calculationDate = "2020-01-01";
                    x = "200000";
                    r = "12";
                    n = "2";
                    method = "2";
                    break;
                case ConsoleKey.D2:
                    Console.WriteLine("Please, input agreement date: ");
                    agreementDate = Console.ReadLine() ?? "";
                    Console.WriteLine("Please, input calculation date: ");
                    calculationDate = Console.ReadLine() ?? "";
                    Console.WriteLine("Please, input x: ");
                    x = Console.ReadLine() ?? "";
                    Console.WriteLine("Please, input R: ");
                    r = Console.ReadLine() ?? "";
                    Console.WriteLine("Please, input N: ");
                    n = Console.ReadLine() ?? "";
                    Console.WriteLine("Please, select method (1 - Differentiated, 2 - Annuity): ");
                    method = Console.ReadLine() ?? "";
                    break;
                default:
                    return;
            }

            var input = new InputModel()
            {
                AgreementDate = DateTime.Parse(agreementDate),
                CalculationDate = DateTime.Parse(calculationDate),
                SumOfCredit = Double.Parse(x),
                PercentByYear = Double.Parse(r),
                Years = Double.Parse(n),
            };
            
            var result = calculation.CalculateLeftPayments(input, Int32.Parse(method));
            Console.WriteLine($"Percent part left: {result}");
        }
    }

    public class InputModel
    {
        public DateTime AgreementDate { get; init; }
        public DateTime CalculationDate { get; init; }
        public double SumOfCredit { get; init; }
        public double PercentByYear { get; init; }
        public double Years { get; init; }
    }

    public interface ICalculation
    {
        public double? CalculateLeftPayments(InputModel inputModel, int method);
    }

    public class Calculation : ICalculation
    {
        public double? CalculateLeftPayments(InputModel inputModel, int method)
        {
            Console.WriteLine("*** NOTE ***");
            Console.WriteLine("Day for monthly payment is the day of agreement date");
            Console.WriteLine("If agreement date does not exist for calculation,\n" +
                              "payment day for this month is 1st of next month");
            Console.WriteLine("*** END NOTE ***\n");
            switch (method)
            {
                case 1:
                    return DifferentiatedCalculationLeftPayments(inputModel);
                case 2:
                    return AnnuityCalculationLeftPayments(inputModel);
                default:
                    return null;
            }
        }

        private double DifferentiatedCalculationLeftPayments(InputModel inputModel)
        {
            if (inputModel.Years <= 0 || inputModel.SumOfCredit < 0 || inputModel.PercentByYear < 0)
            {
                throw new ArgumentException();
            }

            var totalLeft = inputModel.SumOfCredit;
            var totalMonths = inputModel.Years * 12;
            var percentPerMonth = inputModel.PercentByYear / 100 / 12;
            var creditPartPerMonth = inputModel.SumOfCredit / totalMonths;

            var paymentsDone = inputModel.CalculationDate.Month - inputModel.AgreementDate.Month +
                               12 * (inputModel.CalculationDate.Year - inputModel.AgreementDate.Year) +
                               (inputModel.AgreementDate.Day < inputModel.CalculationDate.Day ? 0 : -1);

            var sum = 0.0;
            for (var i = 0; i < totalMonths; i++)
            {
                if (i >= paymentsDone)
                {
                    var percentPartPerMonth = totalLeft * percentPerMonth;
                    sum += percentPartPerMonth;
                }

                totalLeft -= creditPartPerMonth;
            }

            return sum;
        }

        private double AnnuityCalculationLeftPayments(InputModel inputModel)
        {
            if (inputModel.Years <= 0 || inputModel.SumOfCredit < 0 || inputModel.PercentByYear < 0)
            {
                throw new ArgumentException();
            }

            var totalLeft = inputModel.SumOfCredit;
            var totalMonths = inputModel.Years * 12;
            var percentPerMonth = inputModel.PercentByYear / 100 / 12;
            var tempSum = inputModel.SumOfCredit * (percentPerMonth + percentPerMonth / (Math.Pow(1 + percentPerMonth, totalMonths) - 1));

            var paymentsDone = inputModel.CalculationDate.Month - inputModel.CalculationDate.Month +
                               12 * (inputModel.CalculationDate.Year - inputModel.AgreementDate.Year) +
                               (inputModel.AgreementDate.Day < inputModel.CalculationDate.Day ? 0 : -1);

            var sum = 0.0;
            for (var i = 0; i < totalMonths; i++)
            {
                if (i >= paymentsDone)
                {
                    sum += totalLeft * percentPerMonth;
                }

                totalLeft -= tempSum - totalLeft * percentPerMonth;
            }

            return sum;
        }
    }
}