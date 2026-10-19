using Sandbox;
using System.Globalization;
using System.Linq;

namespace Sandbox
{
    public class PartRecipe
    {
        public int recipeID;
        public string partName;
        public double nominalSize;
        public double goodTolerance;
        public double emgTolerance;
        public bool haveRecipe;
        public PartRecipe(int RecipeID)
        {
            recipeID = RecipeID;
            if (RecipeID == 1)
            {
                partName = "Втулка";
                nominalSize = 50;
                goodTolerance = 2.0;
                emgTolerance = 15.0;
                haveRecipe = true;
            }
            else if (RecipeID == 2)
            {
                partName = "Вал";
                nominalSize = 120;
                goodTolerance = 2.0;
                emgTolerance = 15.0;
                haveRecipe = true;
            } 
            else
            {
                haveRecipe = false;
            }
        }
    }
    public class PartReport
    {
        public int ID { get; set; }
        public string PartName { get; set; }
        public double Size { get; set; }
        public ConveyorState Status { get; set; }
        public double Deviation { get; set; }
    }
    public enum ConveyorState
    {
        Idle,
        Scanning,
        Accepted,
        Rejected,
        Emergency,
    }
    public class ConveyorController
    {
        public int LineId { get; set; }
        public ConveyorState CurrentState { get; private set; }
        public List<PartReport> History { get; private set; }
        public double GetAverageSize() => History.Count > 0 ? History.Average(p => p.Size) : 0;
        public double GetMaxDeviation() => History.Count > 0 ? History.Max(p => Math.Abs(p.Deviation)) : 0;
        public int GetGoodParts() => History.Count(p => p.Status == ConveyorState.Accepted);
        public bool HasEmg() => History.Any(p => p.Status == ConveyorState.Emergency);
        public int GetRejectParts() => History.Count(p => p.Status == ConveyorState.Rejected || p.Status == ConveyorState.Emergency);
        public ConveyorController(int lineID)
        {
            LineId = lineID;
            CurrentState = ConveyorState.Idle;
            History = new List<PartReport>();
        }
        public void ProcessPart(int partId, double detalSize, int recipeID)
        {
            PartRecipe recipe = new PartRecipe(recipeID);
            CurrentState = ConveyorState.Scanning;
            if ( detalSize >= (recipe.nominalSize - recipe.goodTolerance) && detalSize <= (recipe.nominalSize + recipe.goodTolerance))
            {
                CurrentState = ConveyorState.Accepted;
            }
            else if (detalSize >= (recipe.nominalSize - recipe.emgTolerance) && detalSize <= (recipe.nominalSize + recipe.emgTolerance))
            {
                CurrentState = ConveyorState.Rejected;
            }
            else
            {
                CurrentState = ConveyorState.Emergency;
            }
            History.Add(new PartReport
            {
                ID = partId,
                PartName = recipe.partName,
                Size = detalSize,
                Status = CurrentState,
                Deviation = detalSize - recipe.nominalSize
            });
        }
    }
    internal class Program
    {
        public static void Main(string[] args)
    {
        Console.WriteLine("Среда готова к работе.");
        while (true)
        {
            int ProgNumb = ReadInt("Введите программу для работы (0 - Завершение работы): ");
            switch (ProgNumb)
            {
                case 0:
                    Console.WriteLine("Завершение работы программы");
                    Console.WriteLine();
                    return;
                case 1:
                    Console.WriteLine("[OK] Номер программы принят");
                    Console.WriteLine();
                    RunCoordinateCheck();
                    break;

                case 2:
                    Console.WriteLine($"[OK] Номер программы принят");
                    Console.WriteLine();
                    RunSpeedCalculation();
                    break;

                case 3:
                    Console.WriteLine($"[OK] Номер программы принят");
                    Console.WriteLine();
                    RunJointMonitor();
                    break;

                case 4:
                    Console.WriteLine($"[OK] Номер программы принят");
                    Console.WriteLine();
                    RunConveyorSimulation();
                    break;

                default:
                    Console.WriteLine("Такой программмы несуществует, повторите ввод");
                    break;
            }
        }
    }
        public static double CheckAngle(double min, double max, int currentJoint)
    {
        while (true)
        {
            double angle = ReadDouble($"Введите ось A{currentJoint + 1}: ");
            if (angle < min || angle > max)
            {
                Console.WriteLine("[АВАРИЯ] Выход за пределы оси, повторите ввод");
                Console.WriteLine();
            }
            else
            {
                return angle;
            }
        }
    }
        public static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            string normalInput = input.Replace(',', '.');
            if (double.TryParse(normalInput, CultureInfo.InvariantCulture, out double doubleoutput))
            {
                return doubleoutput;
            }
            else
            {
                Console.WriteLine("[ОШИБКА] Некоректный формат ввода, повторите");
                Console.WriteLine();
            }
        }
    }
        public static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (int.TryParse(input, out int intoutput))
            {
                return intoutput;
            }
            else
            {
                Console.WriteLine("[ОШИБКА] Некоректный формат ввода, повторите");
                Console.WriteLine();
            }
        }
    }
        public static void RunCoordinateCheck()
    {
        double targetX = ReadDouble("Введите кординату Х в мм: ");
        if (Math.Abs(targetX) <= 500.0)
        {
            Console.WriteLine($"[ОК] Координата принята: {targetX:F2} мм");
        }
        else
        {
            if (targetX < -500.0)
            {
                double incorrectX = targetX + 500.0;
                Console.WriteLine($"[АВАРИЯ] Выход за пределы рабочей зоны на: {incorrectX} мм");
                Console.WriteLine();
            }
            else
            {
                double incorrectX = targetX - 500.0;
                Console.WriteLine($"[АВАРИЯ] Выход за пределы рабочей зоны на: {incorrectX} мм");
                Console.WriteLine();
            }
        }
        Console.WriteLine();
    }
        public static void RunSpeedCalculation()
    {
        double distance = ReadDouble("Введите дистанцию в мм: ");
        double speed = ReadDouble("Введите скорость в мм/сек: ");
        if ((speed <= 0) || (distance <= 0))
        {
            Console.WriteLine($"[ОШИБКА] Введены некоректные данные");
            Console.WriteLine();
        }
        else
        {
            double time = distance / speed;
            Console.WriteLine($"Время расчитано: {time} сек");
        }
        Console.WriteLine();
    }
        public static void RunJointMonitor()
    {
        double[] min = [-180, -50, -90, -180, -90, -360];
        double[] max = [180, 120, 140, 180, 90, 360];
        double[] jointAngles = new double[6];
        int numberOfJoints = 6;
        for (int currentJoint = 0; currentJoint < numberOfJoints; currentJoint++)
        {
            double angle = CheckAngle(min[currentJoint], max[currentJoint], currentJoint);
            jointAngles[currentJoint] = angle;
            Console.WriteLine($"Принято ось А{currentJoint + 1} = {angle}");
            Console.WriteLine();
        }
        Console.WriteLine($"--- СОСТОЯНИЕ ОСЕЙ ---\nA1 = {jointAngles[0]};\nA2 = {jointAngles[1]};\nA3 = {jointAngles[2]};\nA4 = {jointAngles[3]};\nA5 = {jointAngles[4]};\nA6 = {jointAngles[5]};");
        int jointsInPlus = 0;
        int maxIncorrectJoint = 0;
        for (int currentJointN2 = 1; currentJointN2 < 6; currentJointN2++)
        {
            if (Math.Abs(jointAngles[maxIncorrectJoint]) < Math.Abs(jointAngles[currentJointN2]))
            {
                maxIncorrectJoint = currentJointN2;
            }
        }
        bool[] kolIncorrect = [false, false, false, false, false, false];
        for (int currentJointN3 = 0; currentJointN3 < 6; currentJointN3++)
        {
            if (Math.Abs(jointAngles[maxIncorrectJoint]) == Math.Abs(jointAngles[currentJointN3]))
            {
                kolIncorrect[currentJointN3] = true;
            }
            else
            {
                kolIncorrect[currentJointN3] = false;
            }
            if (jointAngles[currentJointN3] >= 0)
            {
                jointsInPlus++;
            }
            if (kolIncorrect[currentJointN3] == true)
            {
                Console.WriteLine($"Наибольшее отклонение: Ось А{currentJointN3 + 1} ({jointAngles[currentJointN3]}°)");
            }
        }
        Console.WriteLine($"Количество осей в плюсовой зоне: {jointsInPlus}");
        Console.WriteLine();
    }
        public static void RunConveyorSimulation()
        {
            int conveyorID = ReadInt("[OK] Введите ID конвейера: ");
            ConveyorController conveyor = new ConveyorController(conveyorID);
            if (conveyor.CurrentState != ConveyorState.Idle)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[ОШИБКА] Конвейер не запустился!");
                Console.ResetColor();
                return;
            }
            Console.WriteLine("[OK] Конвейер запущен");
            int recipID = ReadInt("Введите ID рецепта\n(1 - Втулка,\n2 - Вал): ");
            PartRecipe recipe = new PartRecipe(recipID);
            while (true)
            {
                if (recipe.haveRecipe)
                {
                    Console.WriteLine($"[OK] Рецепт выбран: {recipe.partName}");
                    break;
                }
                else
                {
                    Console.WriteLine("[ОШИБКА] Несуществующий рецепт, повторите ввод");
                    Console.WriteLine();
                    recipID = ReadInt("Введите ID рецепта\n(1 - Втулка,\n2 - Вал: ");
                }
            }
            
            Console.WriteLine("[OK] Ожидание детали");
            Console.WriteLine();
            for (int part = 1; part <= 5; part++)
            {
                Console.WriteLine($"[OK] Сканирование детали {part}");
                Console.WriteLine();
                double detalSize = ReadDouble("Запрос размера детали: ");
                conveyor.ProcessPart(part, detalSize, recipe.recipeID);
                if (conveyor.CurrentState == ConveyorState.Accepted)
                {
                    Console.WriteLine($"[OK] Деталь {part} принята");
                    Console.WriteLine();
                }
                else if (conveyor.CurrentState == ConveyorState.Rejected)
                {
                    Console.WriteLine($"[DROP] Деталь {part} сброшена в корзину брака");
                    Console.WriteLine();
                }
                else if (conveyor.CurrentState == ConveyorState.Emergency)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ALARM] Критический размер! Аварийный останов линии!");
                    Console.ResetColor();
                    Console.WriteLine();
                    break;
                }
            }
            int rejectCount = 0;
            foreach (PartReport report in conveyor.History)
            {
                string sign = report.Deviation >= 0 ? "+" : "";
                Console.WriteLine($"Деталь #{report.ID:D2} ({report.PartName}): Размер = {report.Size:F2}мм | Отклонение = {sign}{report.Deviation:F2}мм | Статус: {report.Status}");
                if (report.Status == ConveyorState.Rejected || report.Status == ConveyorState.Emergency)
                {
                    rejectCount++;
                }
            }
            Console.WriteLine($"Всего проверено деталей: {conveyor.History.Count:D1} шт\nБракованных деталей: {conveyor.GetRejectParts():D1} шт\nСредний размер: {conveyor.GetAverageSize():F2} мм\nМаксимальное отклонение: {conveyor.GetMaxDeviation():F2} мм\nКоличество хороших деталей {conveyor.GetGoodParts():D1} шт");
            if (conveyor.HasEmg())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ВНИМАНИЕ] Партия прервана по аварии!");
                Console.ResetColor();
            }
        
        }
    }
}

