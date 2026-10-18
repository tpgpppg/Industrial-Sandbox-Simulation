using System.Globalization;

namespace Sandbox
{

    internal class Program
    {
        public class PartReport
        {
            public int ID { get; set; }
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
        public static void Main(string[] args)
        {
            Console.WriteLine("Среда готова к работе.");
           
            while (true)
            {
                double ProgNumb = ReadDouble("Введите программу для работы (0 - Завершение работы): ");
                switch (ProgNumb)
                {
                    case 0:
                        Console.WriteLine("Завершение работы программы");
                        Console.WriteLine();
                        return;
                    case 1:
                        Console.WriteLine("[OK] Номер программы принят");
                        Console.WriteLine();
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
                        break;

                    case 2:
                        Console.WriteLine($"[OK] Номер программы принят");
                        Console.WriteLine();

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
                        break;

                    case 3:
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
                        for (int currentJointN2 = 1; currentJointN2 < 6; currentJointN2++ )
                        {
                            if (Math.Abs(jointAngles[maxIncorrectJoint]) < Math.Abs(jointAngles[currentJointN2]))
                            {
                                maxIncorrectJoint = currentJointN2;
                            }
                        }
                        bool[] kolIncorrect = [false, false, false, false , false, false];
                        for(int currentJointN3 = 0; currentJointN3 < 6; currentJointN3++)
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
                        break;

                    case 4:
                        List<PartReport> bathHistory = new List<PartReport>();
                        bool DO_dropper = false;
                        ConveyorState currentState = ConveyorState.Idle;
                        Console.WriteLine(currentState);
                        Console.WriteLine("[OK] Ожидание детали");
                        Console.WriteLine();
                        for(int part = 1; part <= 5; part++)
                        {
                        DO_dropper = false;
                        currentState = ConveyorState.Scanning;
                        Console.WriteLine(currentState);
                        Console.WriteLine($"[OK] Сканирование детали {part}");
                        Console.WriteLine();
                        double detalSize = ReadDouble("Запрос размера детали: ");
                        double deviation = detalSize - 50.0; 
                            if (detalSize >= 49.0 && detalSize <= 51.0)
                            {
                                currentState = ConveyorState.Accepted;
                                Console.WriteLine(currentState);
                                Console.WriteLine($"[OK] Деталь {part} принята");
                                Console.WriteLine();
                            }
                            else if (detalSize >= 40.0 && detalSize <= 60.0)
                            {
                                DO_dropper = true;
                                currentState = ConveyorState.Rejected;
                                Console.WriteLine(currentState);
                                Console.WriteLine($"[DROP] Деталь {part} сброшена в корзину брака");
                                Console.WriteLine();
                            }
                            else
                            {
                                currentState = ConveyorState.Emergency;
                                Console.WriteLine(currentState);
                                Console.WriteLine($"[ALARM] Критический размер! Аварийный останов линии!");
                                Console.WriteLine();
                            }
                            
                            bathHistory.Add(new PartReport { ID = part, Size = detalSize, Status = currentState, Deviation = deviation});
                            if (currentState == ConveyorState.Emergency)
                            {
                                break;
                            }
                        }
                        int rejectCount = 0;
                        foreach (PartReport report in bathHistory)
                        {
                            string sign = report.Deviation >= 0 ? "+" : "";
                            Console.WriteLine($"Деталь #{report.ID:D2}: Размер = {report.Size:F2}мм | Отклонение = {sign}{report.Deviation:F2}мм | Статус: {report.Status} ");
                            if (report.Status == ConveyorState.Rejected || report.Status == ConveyorState.Emergency)
                            {
                                rejectCount++;
                            }
                        }
                        Console.WriteLine($"Всего проверено деталей: {bathHistory.Count}\nБракованных деталей: {rejectCount}");
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
                if (double.TryParse(normalInput, CultureInfo.InvariantCulture, out double output))
                {
                    return output;
                }
                else
                {
                    Console.WriteLine("[ОШИБКА] Некоректный формат ввода, повторите");
                    Console.WriteLine();
                }
            }
        }
    }
}
