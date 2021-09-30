using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Runtime.InteropServices;

namespace MyLang
{
    public partial class Form1 : Form
    {
        int line;
        int row;
        bool clearSelection = false;
        Dictionary<string, string> variables;
        Dictionary<int, List<string>> words;
        int countElementsInList;
        List<string> reservedWords = new List<string> { "знаки", "анализ", "синтез" };
        bool opredeleniePassed;
        bool operatorPassed;
        bool mnojestvoPassed;
        bool resWord;
        int index;

        public enum ScrollBarType : uint
        {
            SbHorz = 0,
            SbVert = 1,
            SbCtl = 2,
            SbBoth = 3
        }

        public enum Message : uint
        {
            WM_VSCROLL = 0x0115
        }

        public enum ScrollBarCommands : uint
        {
            SB_THUMBPOSITION = 4
        }

        [DllImport("User32.dll")]
        public extern static int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("User32.dll")]
        public extern static int SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tb_Example.Text = "Язык = Определение...Определение Опер...Опер Множество \";\"...\r\nМножество " +
                "\r\nОпределение = \"Знаки\" Вещ\";\"...Вещ" +
                "\r\nОпер = </Метка...Метка\":\"/> Перем \"=\" Прав.часть" +
                "\r\nМножество = [\"Анализ\"!\"Синтез\"] Цел \",\" ... Цел\r\n" +
                "Прав.часть = </\"-\"/> Блок1 Зн1 ... Блок1\r\n" +
                "Зн1 = \"+\" ! \"-\"\r\n" +
                "Блок1 = Блок2 Зн2 ... Блок2\r\n " +
                "Зн2 = \"*\" ! \"/\"\r\n" +
                "Блок2 = Блок3 Зн3 ... Блок3\r\n" +
                "Зн3 = \"&\" ! \"|\" \r\n" +
                "Блок3 = </\"~\"/> Блок4\r\n" +
                "Блок4 = Перем ! Вещ ! \"(\"Прав.часть\")\" ! \"[\"Прав.часть\"]\"n=2\r\n" +
                "Симв = Бук ! Циф\r\n" +
                "Перем = Бук </Симв ... Симв/>\r\n" +
                "Метка = Цел\r\n" +
                "Цел = Циф ... Циф\r\n" +
                "Вещ = Цел \".\" Цел\r\n" +
                "Бук = \"А\" ! \"Б\" ! \"В\" ! ... \"Я\"\r\n" +
                "Циф = \"0\" ! \"1\" ! \"2\" ! ... \"D\" ! \"E\" ! \"F\"";
        }

        private void New_Line()
        {
            tb_Debug.Clear();
            tb_Strings.Clear();
            line = tb_Program.Lines.Count();
            for(int i = 1; i<=line; i++)
            {
                tb_Strings.AppendText(i.ToString() + "\r\n");
            }
        }

        private void tb_Program_TextChanged_1(object sender, EventArgs e)
        {
            New_Line();
            
        }
        private void tb_Program_MouseDown(object sender, MouseEventArgs e)
        {
            if (clearSelection)
            {
                tb_Program.SelectAll();
                tb_Program.SelectionColor = System.Drawing.Color.Black;
                tb_Program.SelectionBackColor = System.Drawing.Color.White;
                tb_Program.DeselectAll();
                clearSelection = false;
            }
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            int lines = tb_Program.Lines.Count();
            tb_Debug.Clear();
            opredeleniePassed = false;
            operatorPassed = false;
            mnojestvoPassed = false;
            resWord = false;
            index = 0;
            if (lines == 0)
            {
                MessageBox.Show("Ошибка! На вход подан пустой текст!");
                return;
            }
            else
            {
                PreDebugProgram();
            }

        }

        private void OutputError(string errorMessage, List<string> mString, int nStr, int range, int selectionLength)
        {
            int selectionStart = tb_Program.GetFirstCharIndexFromLine(nStr-1);

            for (int i = 0; i < range; i++)
            {
                selectionStart = tb_Program.Find(mString[i], selectionStart + (i == 0 ? 0 : 1), RichTextBoxFinds.MatchCase);
            }
            tb_Program.Select(selectionStart, selectionLength);
            tb_Program.SelectionColor = System.Drawing.Color.Black;
            tb_Program.SelectionBackColor = System.Drawing.Color.Red;
            clearSelection = true;

            tb_Debug.Text = errorMessage;
            return;

        }

        private void selectError(int start, int length)
        {
            tb_Program.Select(start, length);
            tb_Program.SelectionColor = System.Drawing.Color.Black;
            tb_Program.SelectionBackColor = System.Drawing.Color.Red;
            clearSelection = true;
        }

        private void PreDebugProgram()
        {
            if(words != null)
                words.Clear();
            words = new Dictionary<int, List<string>>();
            if (variables != null)
                variables.Clear();
            variables = new Dictionary<string, string>();
            row = 1;
            foreach (var line in tb_Program.Lines)
            {
                string code = line.Replace(",", " , ")
                    .Replace(":", " : ")
                    .Replace(";", " ; ")
                    .Replace("=", " = ")
                    .Replace("\r\n", " ")
                    .Replace("*", " * ")
                    .Replace("/", " / ")
                    .Replace("-", " - ")
                    .Replace("+", " + ")
                    .Replace("(", " ( ")
                    .Replace(")", " ) ")
                    .Replace("[", " [ ")
                    .Replace("]", " ] ")
                    .Replace("|", " | ")
                    .Replace("~", " ~ ")
                    .Replace("&", " & ")
                    .Replace(".", " . ");

                if (line.Contains(". "))
                {
                    int selectionStart = tb_Program.GetFirstCharIndexFromLine(row);
                    for (int i = 0; i < line.Length; i++)
                    {
                        selectionStart = tb_Program.Find(". ");
                    }
                    selectError(selectionStart, 2);
                    tb_Debug.Text = "Ошибка! Строка " + row + ": После точки ожидалась целое 16-ричное число!";
                    return;
                }
                else if(line.Contains(" ."))
                {
                    int selectionStart = tb_Program.GetFirstCharIndexFromLine(row);
                    for (int i = 0; i < line.Length; i++)
                    {
                        selectionStart = tb_Program.Find(" .");
                    }
                    selectError(selectionStart, 2);
                    tb_Debug.Text = "Ошибка! Строка " + row + ": После целого 16-ричного числа ожидалась точка!";
                    return;
                }
                code = Regex.Replace(code, @"\s{2,}", " ", RegexOptions.None);
                code = code.Trim();
                words.Add(row, code.Split(' ').ToList());
                row++;
            }
            DebugProgram();
        }

        private void DebugProgram()
        {
            foreach(var record in words)
            {
                index++;
                countElementsInList = record.Value.Count;
                if (record.Value[0] == "")
                    continue;
                //проверка что знаки в начале
                if (record.Value[0] == "Знаки")
                {
                    bool semicolon = false;
                    
                    if (record.Value.Count == 1)
                    {
                        OutputError("Ошибка! Строка " + record.Key.ToString() + ": После слова \"Знаки\" ожидалось 16-ричное вещественное число!",
                            record.Value, record.Key, countElementsInList, record.Value[0].Length);
                        return;
                    }
                    for (int number = 1; number <= countElementsInList - 1; number+=4)
                    {
                        for (int sym = 0; sym < 4; sym++)
                        {
                            if(sym == 0 && semicolon == false)
                            {
                                if(!Regex.IsMatch(record.Value[number+sym], @"^[\da-f]+$",RegexOptions.IgnoreCase))
                                {
                                    OutputError("Ошибка! Строка " + record.Key.ToString() + ": После слова \"Знаки\" ожидалось 16-ричное вещественное число!",
                                        record.Value, record.Key, number + sym, record.Value[number + sym].Length);
                                    return;
                                }
                            }
                            else if (sym == 0 && semicolon == true)
                            {
                                if (!Regex.IsMatch(record.Value[number + sym], @"^[\da-f]+$", RegexOptions.IgnoreCase))
                                {
                                    OutputError("Ошибка! Строка " + record.Key.ToString() + ": После ; ожидалось 16-ричное вещественное число !",
                                        record.Value, record.Key, number + sym, record.Value[number + sym].Length);
                                    return;
                                }
                            }
                            else if (sym == 1 && number!=countElementsInList - 1)
                            {
                                if(!Regex.IsMatch(record.Value[number+sym],@"\."))
                                {
                                    OutputError("Ошибка! Строка " + record.Key.ToString() + ": После " + record.Value[number + sym - 1] + " ожидалась точка!",
                                        record.Value, record.Key, number + sym, record.Value[number + sym].Length);
                                    return;
                                }
                            }
                            else if (sym == 1 && number == countElementsInList - 1)
                            {
                                OutputError("Ошибка! Строка " + record.Key.ToString() + ": После " + record.Value[number + sym - 1] + " ожидалась точка!",
                                       record.Value, record.Key, number + sym, record.Value[number + sym-1].Length);
                                return;
                            }
                            else if(sym == 2 && number!=countElementsInList - 2)
                            {
                                if (!Regex.IsMatch(record.Value[number + sym], @"^[\da-f]+$", RegexOptions.IgnoreCase))
                                {
                                    OutputError("Ошибка! Строка " + record.Key.ToString() + ": После точки ожидалось 16-ричное число!",
                                       record.Value, record.Key, number + sym, record.Value[number + sym].Length);

                                    return;
                                }
                            }
                            else if (sym == 2 && number == countElementsInList - 2)
                            {
                                OutputError("Ошибка! Строка " + record.Key.ToString() + ": После точки ожидалось 16-ричное число!",
                                       record.Value, record.Key, number + sym, record.Value[number + sym-1].Length);
                                return;
                            }
                            else if(sym == 3 && number+3!=countElementsInList)
                            {
                                if(Regex.IsMatch(record.Value[number+sym],@"\."))
                                {
                                    OutputError("Ошибка! Строка " + record.Key.ToString() + ": После точки ожидалось 16-ричное число!",
                                       record.Value, record.Key, number + sym, record.Value[number + sym].Length);
                                    return;
                                }
                                if (!Regex.IsMatch(record.Value[number + sym], @";"))
                                {
                                    OutputError("Ошибка! Строка " + record.Key.ToString() + ": После " + record.Value[number] + record.Value[number + 1] + record.Value[number + 2] + " ожидалась \";\" !",
                                       record.Value, record.Key, number + sym, record.Value[number + sym].Length);
                                   return;
                                }
                                semicolon = true;
                            }
                        }

                        
                    }
                    if (record.Value[countElementsInList - 1] == ";")
                    {
                        OutputError("Ошибка! Строка " + record.Key.ToString() + ": После " + record.Value[countElementsInList - 4] + record.Value[countElementsInList - 3] + record.Value[countElementsInList - 2] + " ожидалось слово \"Знаки\" , метка или переменная!",
                                   record.Value, record.Key, countElementsInList, record.Value[countElementsInList - 1].Length);
                        return;
                    }
                    opredeleniePassed = true;
                    continue;
                }
                else if (record.Value[0] != "Знаки" && !opredeleniePassed)
                {
                    OutputError("Ошибка! Строка " + record.Key.ToString() + ": Программа должна начинаться со слова \"Знаки\"!",
                                   record.Value, record.Key, 1, record.Value[0].Length);
                    return;
                }
                //проверка переменных и меток
                else if(opredeleniePassed && !mnojestvoPassed)
                {
                    int colonNum = 0;
                    //проверка когда есть метки
                    if(Regex.IsMatch(record.Value[0],@"^[\da-f]+$",RegexOptions.IgnoreCase))
                    {
                        //проверка метки
                        for(int i = 1; i<countElementsInList; i++)
                        {
                            if (record.Value[i] == ":")
                            {
                                colonNum = i;
                                break;
                            }

                            if (!Regex.IsMatch(record.Value[i], @"^[\da-f]+$"))
                            {
                                OutputError("Ошибка! Строка " + record.Key.ToString() + ": После " + record.Value[i - 1] + " ожидалось : или метка (целое 16-ричное число) !",
                                   record.Value, record.Key, countElementsInList, record.Value[i-1].Length);
                                return;
                            }
                        }
                        if(colonNum == 0)
                        {
                            OutputError("Ошибка! Строка " + record.Key.ToString() + ": После " + record.Value[countElementsInList - 1] + " ожидалось : или метка (целое 16-ричное число) !",
                                   record.Value, record.Key, countElementsInList, record.Value[countElementsInList-1].Length);
                            return;
                        }
                        //проверка переменной
                        if (colonNum != countElementsInList - 1)
                        {
                            if (!Regex.IsMatch(record.Value[colonNum + 1], @"^[а-яё]+\d*$", RegexOptions.IgnoreCase))
                            {
                                if (Regex.IsMatch(record.Value[colonNum + 1], @"^[a-z]+\d*$", RegexOptions.IgnoreCase))
                                {
                                    OutputError("Ошибка! Строка " + record.Key.ToString() + ": Переменная объявлена латиницей!",
                                        record.Value, record.Key, colonNum+2, record.Value[colonNum+1].Length);
                                    return;
                                }
                                if (Regex.IsMatch(record.Value[colonNum + 1], @"^\d+[a-zа-яё]*$", RegexOptions.IgnoreCase))
                                {
                                    OutputError("Ошибка! Строка " + record.Key.ToString() + ": Переменная объявлена латиницей!",
                                        record.Value, record.Key, colonNum + 2, record.Value[colonNum + 1].Length);
                                    return;
                                }
                                OutputError("Ошибка! Строка " + record.Key.ToString() + ": После : ожидалась переменная!",
                                        record.Value, record.Key, colonNum + 2, record.Value[colonNum+1].Length);
                                return;
                            }
                            else if (reservedWords.Contains(record.Value[colonNum + 1].ToLower()))
                            {
                                OutputError("Ошибка! Строка " + record.Key.ToString() + ": Переменная задана зарезервированным словом \"" + record.Value[colonNum + 1] + "\" !",
                                        record.Value, record.Key, colonNum + 2, record.Value[colonNum + 1].Length);
                                return;
                            }
                            else
                            {
                                if (colonNum + 1 != countElementsInList - 1)
                                {
                                    int equalNum = colonNum + 2;
                                    if (record.Value[equalNum] == "=" && equalNum != countElementsInList - 1)
                                    {
                                        List<string> varVal = new List<string>();
                                        for (int i = equalNum + 1; i < countElementsInList; i++)
                                        {
                                            if (Regex.IsMatch(record.Value[i], @"^[\da-f]+$", RegexOptions.IgnoreCase))
                                            {
                                                if (i + 2 < countElementsInList)
                                                {
                                                    string tmp = record.Value[i];
                                                    if (record.Value[i + 1] == ".")
                                                    {
                                                        if (Regex.IsMatch(record.Value[i + 2], @"^[\da-f]+$", RegexOptions.IgnoreCase))
                                                        {
                                                            tmp += "." + record.Value[i + 2];
                                                            varVal.Add(tmp);
                                                            i += 2;
                                                            continue;
                                                        }
                                                        else
                                                        {
                                                            OutputError("Ошибка! Строка " + record.Key.ToString() + ": После точки ожидалось 16-ричное число!",
                                                                record.Value, record.Key, i+2, record.Value[i].Length);
                                                            return;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        OutputError("Ошибка! Строка " + record.Key.ToString() + ": После 16-ричного числа ожидалась точка!",
                                                               record.Value, record.Key, i+1, record.Value[i].Length);
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    OutputError("Ошибка! Строка " + record.Key.ToString() + ": Ожидалось 16-ричное вещественное число!",
                                                               record.Value, record.Key, countElementsInList, record.Value[i].Length);
                                                   return;
                                                }
                                            }
                                            varVal.Add(record.Value[i]);
                                        }

                                        string answer = CheckMath(record.Key, varVal);

                                        if (answer == string.Empty)
                                        {
                                            return;
                                        }
                                        if(answer == Double.PositiveInfinity.ToString() || answer == Double.NegativeInfinity.ToString())
                                        {
                                            OutputError("Ошибка! Строка " + record.Key.ToString() + ": Деление на 0", varVal, record.Key, varVal.Count, 1);
                                            return;
                                        }
                                        answer = ConvertToHex(answer);
                                        //если имя переменной уже использовалось
                                        if (variables.ContainsKey(record.Value[colonNum+1]))
                                        {
                                            variables[record.Value[colonNum+1]] = answer;
                                        }
                                        //если новая переменная
                                        else
                                        {
                                            variables.Add(record.Value[colonNum+1], answer);
                                        }
                                    }
                                    else
                                    {
                                        OutputError("Ошибка! Строка " + record.Key.ToString() + ": Переменной " + record.Value[colonNum + 1] + " не присвоено значение !",
                                                               record.Value, record.Key, equalNum+1, record.Value[equalNum].Length);
                                        return;
                                    }
                                }
                                else
                                {
                                    OutputError("Ошибка! Строка " + record.Key.ToString() + ": После " + record.Value[colonNum + 1] + " ожидался знак = !",
                                                               record.Value, record.Key, colonNum + 2, record.Value[colonNum+1].Length);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            OutputError("Ошибка! Строка " + record.Key.ToString() + ": После : ожидалась переменная !",
                                                               record.Value, record.Key, colonNum + 1, record.Value[colonNum].Length);
                            return;
                        }
                    }
                    //проверка когда меток нет
                    else if(Regex.IsMatch(record.Value[0],@"^[а-яё]+\d*$",RegexOptions.IgnoreCase))
                    {
                        if (reservedWords.Contains(record.Value[0].ToLower()))
                        {
                            resWord = true;
                        }
                        if (countElementsInList>1 && record.Value[1] == "=")
                        {
                            if(resWord)
                            {
                                OutputError("Ошибка! Строка " + record.Key.ToString() + ": Переменная задана зарезервированным словом \"" + record.Value[0] + "\" !",
                                    record.Value, record.Key, 1, record.Value[0].Length);
                                return;
                            }
                            if(countElementsInList != 2)
                            {
                                List<string> varVal = new List<string>();
                                for (int i = 2; i < countElementsInList; i++)
                                {
                                    if(Regex.IsMatch(record.Value[i],@"^[\da-f]+$",RegexOptions.IgnoreCase))
                                    {
                                        if(i+2<countElementsInList)
                                        {
                                            string tmp = record.Value[i];
                                            if (record.Value[i + 1] == ".")
                                            {
                                                if (Regex.IsMatch(record.Value[i+2], @"^[\da-f]+$", RegexOptions.IgnoreCase))
                                                {
                                                    tmp += "." + record.Value[i + 2];
                                                    varVal.Add(tmp);
                                                    i += 2;
                                                    continue;
                                                }
                                                else
                                                {
                                                    OutputError("Ошибка! Строка " + record.Key.ToString() + ": После точки ожидалось 16-ричное число!",
                                                                record.Value, record.Key, i+2, record.Value[i].Length);
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                OutputError("Ошибка! Строка " + record.Key.ToString() + ": После 16-ричного числа ожидалась точка!",
                                                               record.Value, record.Key, i+1, record.Value[i].Length);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            OutputError("Ошибка! Строка " + record.Key.ToString() + ": Ожидалось 16-ричное вещественное число!",
                                                               record.Value, record.Key, countElementsInList, record.Value[i].Length);
                                            return;
                                        }
                                    }
                                    varVal.Add(record.Value[i]);
                                }
                                string answer = CheckMath(record.Key, varVal);
                                
                                if(answer == string.Empty)
                                {
                                    return;
                                }
                                answer = ConvertToHex(answer);
                                //если имя переменной уже использовалось
                                if (variables.ContainsKey(record.Value[0]))
                                {
                                    variables[record.Value[0]] = answer;
                                }
                                //если новая переменная
                                else
                                {
                                    variables.Add(record.Value[0], answer);
                                }
                            }
                            else
                            {
                                OutputError("Ошибка! Строка " + record.Key.ToString() + ": Переменной " + record.Value[0] + " не присвоено значение !",
                                                               record.Value, record.Key, 1, record.Value[0].Length);
                                return;
                            }
                        }
                        else if(resWord)
                        {  
                            int ret = CheckMnojestvo(record.Value, record.Key);
                            if (ret == 0)
                                return;
                            mnojestvoPassed = true;
                        }
                        else
                        {
                            OutputError("Ошибка! Строка " + record.Key.ToString() + ": После " + record.Value[0] + " ожидался знак = !",
                                                              record.Value, record.Key, 1, record.Value[0].Length);
                            return;
                        }

                    }
                    else if (!Regex.IsMatch(record.Value[0], @"^[а-яё]+\d*$", RegexOptions.IgnoreCase))
                    {
                        if (Regex.IsMatch(record.Value[0], @"^[a-z]+\d*$", RegexOptions.IgnoreCase))
                        {
                            OutputError("Ошибка! Строка " + record.Key.ToString() + ": Переменная объявлена латиницей!",
                                        record.Value, record.Key, 1, record.Value[0].Length);
                            return;
                        }
                        if (Regex.IsMatch(record.Value[0], @"^\d+[a-zа-яё]*$", RegexOptions.IgnoreCase))
                        {
                            OutputError("Ошибка! Строка " + record.Key.ToString() + ": Переменная не может быть задана с цифрой в начале!",
                                        record.Value, record.Key, 1, record.Value[0].Length);
                            return;
                        }
                        OutputError("Ошибка! Строка " + record.Key.ToString() + ": После блока \"Знаки\" ожидалась переменная!",
                                        record.Value, record.Key, 1, record.Value[0].Length);
                        return;
                    }
                    operatorPassed = true;
                }
                //проверка множества
                else if(Regex.IsMatch(record.Value[0],@"^Анализ|Синтез$") && opredeleniePassed && operatorPassed)
                {
                    
                    int ret = CheckMnojestvo(record.Value, record.Key);
                    if (ret == 0)
                        return;
                    mnojestvoPassed = true;
                }
            }
            if(!opredeleniePassed || !operatorPassed || !mnojestvoPassed)
            {
                if(!opredeleniePassed)
                {
                    tb_Debug.Text = "Пропущено \"Определение\"!";
                    return;
                }
                else if (!operatorPassed)
                {
                    tb_Debug.Text = "Пропущен \"Оператор\"!";
                    return;
                }
                else if (!mnojestvoPassed)
                {
                    tb_Debug.Text = "Пропущено \"Множество\"!";
                    return;
                }
            }
            //вывод переменных
            tb_Debug.Text = "Результат работы программы: \r\n";
            foreach(var per in variables)
            {
                tb_Debug.Text += per.Key + " = " + per.Value + "\n";
            }
        }
        private int CheckMnojestvo(List<string> str, int nStr)
        {
            if (str.Count == 1)
            {
                if (str[0] == "Анализ")
                {
                    OutputError("Ошибка! Строка " + nStr.ToString() + ": После слова \"Анализ\" ожидалось 16-ричное целое число!",
                    str, nStr, countElementsInList, str[0].Length);
                    return 0;
                }
                else if (str[0] == "Синтез")
                {
                    OutputError("Ошибка! Строка " + nStr.ToString() + ": После слова \"Синтез\" ожидалось 16-ричное целое число!",
                    str, nStr, countElementsInList, str[0].Length);
                    return 0;
                }
                else
                {
                    OutputError("Ошибка! Строка " + nStr.ToString() + ": Ожидалось слово \"Анализ\", либо \"Синтез\" !",
                    str, nStr, countElementsInList, str[0].Length);
                    return 0;
                }
            }
            bool semicolon = false;

            for (int number = 1; number < countElementsInList; number += 2)
            {
                for (int sym = 0; sym < 2; sym++)
                {
                    if (sym == 0 && !semicolon)
                    {
                        if (!Regex.IsMatch(str[number + sym], @"^[a-f\d]", RegexOptions.IgnoreCase))
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": После " + str[number + sym - 1] + " ожидалось 16-ричное целое число!",
                                str, nStr, number + sym, str[number + sym - 1].Length);
                            return 0;
                        }
                    }
                    else if (sym == 0 && semicolon)
                    {
                        if (!Regex.IsMatch(str[number + sym], @"Анализ|Синтез", RegexOptions.IgnoreCase))
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": После ; ожидалось слово \"Анализ\", либо \"Синтез\", либо конец строки!",
                                str, nStr, number + sym, str[number + sym - 1].Length);
                            return 0;
                        }
                    }
                    else if (sym == 1 && number + sym < countElementsInList)
                    {
                        if (str[number + sym] == ";")
                            semicolon = true;
                        else if (str[number + sym] != ",")
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": После " + str[number + sym - 1] + " ожидалась , либо ; !",
                                str, nStr, number + sym, str[number + sym - 1].Length);
                            return 0;
                        }
                    }
                }
            }
            if(index < words.Count)
            {
                if (semicolon && !Regex.IsMatch(words[index + 1][0], @"Анализ|Синтез"))
                {
                    OutputError("Ошибка! Строка " + nStr.ToString() + ": После ; ожидалось слово \"Анализ\", либо \"Синтез\", либо конец строки!",
                                    str, nStr, str.Count, 1);
                    return 0;
                }
                if (!semicolon && words[index + 1][0] != string.Empty)
                {
                    OutputError("Ошибка! Строка " + nStr.ToString() + ": После " + str[str.Count - 1] + " ожидался конец строки !",
                                        str, nStr, countElementsInList, str[str.Count - 1].Length);
                    return 0;
                }
            }
            else if(index == words.Count)
            {
                if (semicolon)
                {
                    OutputError("Ошибка! Строка " + nStr.ToString() + ": После " + str[str.Count - 1] + " ожидался конец строки !",
                                       str, nStr, countElementsInList, str[str.Count - 1].Length);
                    return 0;
                }
            }
            
                return 1;
        }
        private string ConvertToHex(string number)
        {
            string beforeDot;
            if(number.Contains("."))
            {
                if (number[0] == '-')
                    beforeDot = number.Substring(1, number.IndexOf(".") - 1);
                else
                    beforeDot = number.Substring(0, number.IndexOf("."));
                string afterDot = number.Substring(number.IndexOf(".") + 1);
                beforeDot = Convert.ToInt64(beforeDot).ToString("X");
                afterDot = Convert.ToInt64(afterDot).ToString("X");
                if (number[0] == '-')
                    beforeDot = String.Concat("-", beforeDot);
                return beforeDot + "." + afterDot;
            }
            number = Convert.ToInt64(number).ToString("X");
            return number + ".0";
        }
        private string ConvertFromHex(string number, bool logic)
        {
            string beforeDot;
            if(number[0] == '-')
                beforeDot = number.Substring(1, number.IndexOf(".") - 1);
            else
                beforeDot = number.Substring(0, number.IndexOf("."));
            beforeDot = Convert.ToInt64(beforeDot, 16).ToString();
            if (logic)
            {
                return beforeDot;
            }
            string afterDot = number.Substring(number.IndexOf(".") + 1);
            afterDot = Convert.ToInt64(afterDot, 16).ToString();
            if (number[0] == '-')
                beforeDot = String.Concat("-", beforeDot);
            return beforeDot + "." + afterDot;
        }
        private string CheckMath(int nStr, List<string> mathString)
        {
            bool pMinus = false;
            bool pMathOper = false;
            bool pNot = false;
            bool pAndOr = false;
            bool pOpenCircleBracket = false;
            bool pCloseCircleBracket = false;
            bool pOpenSquareBracket = false;
            bool pCloseSquareBracket = false;
            bool pDigit = false;
            int countCircleBracket = 0;
            int countSquareBracket = 0;
            int[] countInnerBracket = new int[2] {0,0};
            bool logic = false;
            List<string> computeStr = new List<string>();

            if (mathString.Contains("&") || mathString.Contains("|") || mathString.Contains("~"))
                logic = true;
            for(int i = 0; i<mathString.Count;i++)
            {
                string word = mathString[i];
                if (word == "-")
                {
                    if(pMinus || pMathOper || pNot || pAndOr)
                    {
                        OutputError("Ошибка! Строка " + nStr.ToString() + ": Два знака действия подряд!",
                                        mathString, nStr, i+1, mathString[i].Length);
                        return "";
                    }
                    else
                    {
                        pMinus = true;
                        pMathOper = false;
                        pNot = false;
                        pAndOr = false;
                        pCloseCircleBracket = false;
                        pOpenSquareBracket = false;
                        pCloseSquareBracket = false;
                        pDigit = false;
                        pOpenCircleBracket = false;
                        computeStr.Add(word);
                    }
                }
                else if (Regex.IsMatch(word, @"[\*\/\+]"))
                {
                    if (i == 0)
                    {
                        OutputError("Ошибка! Строка " + nStr.ToString() + ": Знак действия в начале выражения!",
                                        mathString, nStr, i+1, mathString[i].Length);
                        return "";
                    }
                    else if (pCloseCircleBracket || pDigit || pCloseSquareBracket)
                    {
                        pMinus = false;
                        pMathOper = true;
                        pNot = false;
                        pAndOr = false;
                        pCloseCircleBracket = false;
                        pOpenSquareBracket = false;
                        pCloseSquareBracket = false;
                        pDigit = false;
                        pOpenCircleBracket = false;
                        computeStr.Add(word);
                    }
                    else
                    {
                        if (pOpenCircleBracket || pOpenSquareBracket)
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Знак открывающей скобки перед знаком действия!",
                                       mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                        else
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Два знака действия подряд!",
                                       mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                    }
                }
                else if (word == "~")
                {
                    if (pMinus || pMathOper || pNot || pAndOr || pDigit)
                    {
                        if (pDigit)
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Число перед логическим отрицанием!",
                                       mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                        else
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Два знака действия подряд!",
                                       mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                    }
                    else
                    {
                        pMinus = false;
                        pMathOper = false;
                        pNot = true;
                        pAndOr = false;
                        pCloseCircleBracket = false;
                        pOpenSquareBracket = false;
                        pCloseSquareBracket = false;
                        pDigit = false;
                        pOpenCircleBracket = false;
                        computeStr.Add(word);
                    }
                }
                else if (word == "&" || word == "|")
                {
                    if (i == 0)
                    {
                        OutputError("Ошибка! Строка " + nStr.ToString() + ": Знак действия в начале выражения!",
                                       mathString, nStr, i+1, mathString[i].Length);
                        return "";
                    }
                    else if (pCloseCircleBracket || pDigit || pCloseSquareBracket)
                    {
                        pMinus = false;
                        pMathOper = false;
                        pNot = false;
                        pAndOr = true;
                        pCloseCircleBracket = false;
                        pOpenSquareBracket = false;
                        pCloseSquareBracket = false;
                        pDigit = false;
                        pOpenCircleBracket = false;
                        computeStr.Add(word);
                    }
                    else
                    {
                        if (pOpenCircleBracket || pOpenSquareBracket)
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Знак отрывающей скобки перед знаком действия!",
                                       mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                        else
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Два знака действия подряд!",
                                       mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                    }
                }
                else if (word == "(")
                {
                    if(pDigit || pCloseCircleBracket || pCloseSquareBracket)
                    {
                        if (pCloseCircleBracket || pCloseSquareBracket)
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Знак закрывающей скобки перед знаком открывающей скобки!",
                                       mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                        else
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Число перед открывающей скобкой!",
                                       mathString, nStr, i+1, mathString[i-1].Length);
                            return "";
                        }
                    }
                    else
                    {
                        pMinus = false;
                        pMathOper = false;
                        pNot = false;
                        pAndOr = false;
                        pCloseCircleBracket = false;
                        pOpenSquareBracket = false;
                        pCloseSquareBracket = false;
                        pDigit = false;
                        pOpenCircleBracket = true;
                        if(countSquareBracket>0)
                            countInnerBracket[countSquareBracket-1]++;
                        countCircleBracket++;
                        computeStr.Add(word);
                    }
                }
                else if (word == ")")
                {
                    if(pOpenCircleBracket || pMinus || pMathOper || pAndOr || pNot || pOpenSquareBracket)
                    {
                        if (pOpenCircleBracket || pOpenSquareBracket)
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Знак отрывающей скобки перед знаком закрывающей скобки!",
                                      mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                        else
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Знак действия перед закрывающей скобкой!",
                                      mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                    }
                    else
                    {
                        pMinus = false;
                        pMathOper = false;
                        pNot = false;
                        pAndOr = false;
                        pOpenCircleBracket = false;
                        pOpenSquareBracket = false;
                        pCloseSquareBracket = false;
                        pDigit = false;
                        pCloseCircleBracket = true;
                        countCircleBracket--;
                        computeStr.Add(word);
                        if(countCircleBracket<0)
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Количество закрывающих круглых скобок больше количества открывающих!",
                                      mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                        if (countSquareBracket > 0)
                            countInnerBracket[countSquareBracket - 1]--;
                    }
                }
                else if (word == "[")
                {
                    if (pDigit || pCloseCircleBracket || pCloseSquareBracket)
                    {
                        if (pCloseCircleBracket || pCloseSquareBracket)
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Знак закрывающей скобки перед знаком открывающей скобки!",
                                     mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                        else
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Число перед открывающей скобкой!",
                                     mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                    }
                    else
                    {
                        pMinus = false;
                        pMathOper = false;
                        pNot = false;
                        pAndOr = false;
                        pCloseCircleBracket = false;
                        pOpenCircleBracket = false;
                        pCloseSquareBracket = false;
                        pDigit = false;
                        pOpenSquareBracket = true;
                        countSquareBracket++;
                        if(countSquareBracket>2)
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Количество открывающих квадратных скобок больше двух!",
                                     mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                        countInnerBracket[countSquareBracket-1] = 0;
                        computeStr.Add("(");
                    }
                }
                else if (word == "]")
                {
                    if (pOpenCircleBracket || pMinus || pMathOper || pAndOr || pNot || pOpenSquareBracket)
                    {
                        if (pOpenCircleBracket || pOpenSquareBracket)
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Знак отрывающей скобки перед знаком закрывающей скобки!",
                                     mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                        else
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Знак действия перед закрывающей скобкой!",
                                     mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                    }
                    else
                    {
                        pMinus = false;
                        pMathOper = false;
                        pNot = false;
                        pAndOr = false;
                        pOpenCircleBracket = false;
                        pOpenSquareBracket = false;
                        pCloseCircleBracket = false;
                        pDigit = false;
                        pCloseSquareBracket = true;
                        countSquareBracket--;
                        if (countSquareBracket < 0)
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Количество закрывающих квадратных скобок больше количества открывающих!",
                                     mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                        if(countInnerBracket[countSquareBracket]!=0)
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Не все внутренние скобки закрыты!",
                                     mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                        computeStr.Add(")");
                    }
                }
                else if (Regex.IsMatch(word, @"^[\da-f]+\.[\da-f]+$",RegexOptions.IgnoreCase))
                {
                    if(pCloseCircleBracket || pCloseSquareBracket || pDigit)
                    {
                        if (pDigit)
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Два числа подряд!",
                                     mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                        else
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Знак закрывающей скобки перед числом!",
                                     mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                    }
                    else
                    {
                        pMinus = false;
                        pMathOper = false;
                        pNot = false;
                        pAndOr = false;
                        pCloseCircleBracket = false;
                        pOpenCircleBracket = false;
                        pCloseSquareBracket = false;
                        pDigit = true;
                        pOpenSquareBracket = false;
                        computeStr.Add(ConvertFromHex(mathString[i], logic));
                    }
                }
                else if (Regex.IsMatch(word, @"^[а-яё]+\d*$"))
                {
                    if (pCloseCircleBracket || pCloseSquareBracket || pDigit)
                    {
                        if (pDigit)
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Число перед переменной!",
                                     mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                        else
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Знак закрывающей скобки перед переменной!",
                                     mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                    }
                    else
                    {
                        if (variables.ContainsKey(word))
                        {
                            string tmp;
                            variables.TryGetValue(word, out tmp);
                            computeStr.Add(ConvertFromHex(tmp, logic));
                            pMinus = false;
                            pMathOper = false;
                            pNot = false;
                            pAndOr = false;
                            pCloseCircleBracket = false;
                            pOpenCircleBracket = false;
                            pCloseSquareBracket = false;
                            pDigit = true;
                            pOpenSquareBracket = false;
                        }
                        else
                        {
                            OutputError("Ошибка! Строка " + nStr.ToString() + ": Переменная " + word + " не объявлена!",
                                     mathString, nStr, i+1, mathString[i].Length);
                            return "";
                        }
                    }
                }
                else
                {
                    OutputError("Ошибка! Строка " + nStr.ToString() + ": Недопустимый символ в объявлении переменной!",
                                     mathString, nStr, i+1, mathString[i].Length);
                    return "";
                }
            }
            if(countCircleBracket>0 || countSquareBracket>0)
            {
                if (countSquareBracket > 0)
                {
                    OutputError("Ошибка! Строка " + nStr.ToString() + ": Количество открывающих квадратных скобок больше количества закрывающих!",
                                     mathString, nStr, mathString.Count - 1, 1);
                    return "";
                }
                else if(countCircleBracket > 0)
                {
                    OutputError("Ошибка! Строка " + nStr.ToString() + ": Количество открывающих круглых скобок больше количества закрывающих!",
                                     mathString, nStr, mathString.Count - 1, 1);
                    return "";
                }
            }
            
            return CSharpScript.EvaluateAsync(String.Join(" ",computeStr.ToArray())).Result.ToString().Replace(',','.');
        }

        private void tb_Program_VScroll(object sender, EventArgs e)
        {
            int nPos = GetScrollPos(tb_Program.Handle, (int)ScrollBarType.SbVert);
            nPos <<= 16;
            uint wParam = (uint)ScrollBarCommands.SB_THUMBPOSITION | (uint)nPos;
            SendMessage(tb_Strings.Handle, (int)Message.WM_VSCROLL, new IntPtr(wParam), new IntPtr(0));

        }

        private void tb_Program_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            int linesCount = tb_Program.Lines.Count();
            tb_Strings.Text = string.Empty;
            if (linesCount == 0)
                return;
            string text = string.Empty;
            for (int i = 1; i < linesCount; i++)
            {
                text = text + i.ToString() + Environment.NewLine;
            }
            text = text + linesCount.ToString();
            tb_Strings.Text = text;
            tb_Program_VScroll(sender, e);

        }
    }
}
