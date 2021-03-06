# Разработка диагностического средства для языка программирования
Цель курсовой работы по дисциплине «Теория языков программирования и методы трансляции» состоит в закреплении и углублении знаний и навыков, полученных при изучении дисциплины. Курсовая работа предполагает выполнение задания повышенной сложности по проектированию, разработке и тестированию программного обеспечения, а также оформлению сопутствующей документации.

Разработать язык программирования согласно предложенной вариантом форме Бэкуса-Наура или предложить свой проблемно-ориентированный язык. Реализовать транслирующее средство, принимающее в качестве входного параметра текст с программой на выбранном языке. Программное обеспечение, обрабатывающее этот язык, должно иметь синтаксический анализатор, проверяющий входной текст на наличие ошибок. В случае обнаружения ошибки, место с ошибкой должно быть подсвечено в программе.

## Описание языка
>Язык = Определение … Определение Опер … Опер Множество «;» … Множество\
Определение = «Знаки» Вещ «;» … Вещ\
Опер = </ Метка … метка «:» /> Перем «=» Прав.часть\
Множество = [«Анализ» ! «Синтез»] Цел «,» … Цел\
В Прав.части – переменные и вещественные числа; соединены аддитивными, мультипликативными и логическими операциями; есть круглые скобки с любой глубиной вложенности и квадратные скобки с глубиной вложенности 2.\
Переменная – набор букв и цифр с первой буквой.\
Метка целочисленная.\
Русский алфавит, шестнадцатеричная арифметика.

## Определение формы Бэкуса-Наура для разрабатываемого языка
>Язык = Определение … Определение Опер … Опер Множество «;» … Множество\
Определение = «Знаки» Вещ «;» … Вещ\
Опер = </ Метка … метка «:» /> Перем «=» Прав.часть\
Множество = [«Анализ» ! «Синтез»] Цел «,» … Цел\
Прав.часть = «–» Блок1 Зн1 … Блок1\
Зн1 = «+» ! «–»\
Блок1 = Блок2 Зн2 … Блок2\
Зн2 = «*» ! «/»\
Блок2 = Блок3 Зн3 … Блок3\
Зн3 = «&» ! «|»\
Блок3 = </ «~» /> Блок4\
Блок4 = Перем ! Вещ ! «(» Прав.часть «)» ! «[» Прав.часть «]» n=2\
Симв = Бук ! Циф\
Перем = Бук </ Симв … Симв />\
Метка = Цел\
Цел = Циф … Циф\
Вещ = Цел «.» Цел\
Бук = «А» ! «Б» ! «В» ! … «Я»\
Циф = «0» ! «1» ! «2» ! … «D» ! «E» ! «F»
