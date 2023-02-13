# Программное обеспечение для автоматизации расчетов предельных режимов с учетом вероятностного характера влияющих факторов.

+ Данное приложение выполняет расчет предельных перетоков активной мощности, используя метод статистических испытаний. Данный подход подразумевает итерационный расчет, где на каждой итерации формируется случайный набор влияющих факторов. После получения массива результатов производится их обработка и вывод на веб-страницу статистических данных и гистограмм.

+ Разработанный алгоритм позволяет сократить трудозатраты специалистов службы электрических режимов АО "СО ЕЭС".

+ Приложение интегрируется с ПК "RastrWin3" для выполнения расчетов установившегося режима электрической сети.

+ Приложение использует PostgreSQL для хранения данных расчетов и данных пользователей.

+ Для разработки бэкенд-части был использован архитектурный стиль "Чистая архитектура".
