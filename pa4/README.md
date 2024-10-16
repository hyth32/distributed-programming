# PA4. Шаблон взаимодействия Publisher-Subscriber (Издатель-Подписчик)

**Цель:** научиться организовывать взаимодейтсвие компонентов РС на основе публикации событий и подписки на них

## Задание

*Задание делается на базе задания PA3*

### События в системе

Необходимо добавить публикацию следующих событий 
1. *RankCalculated* - после вычисления и сохранения в БД значения метрики *rank*;
2. *SimilarityCalculated* - после вычисления и сохранения в БД значения метрики *similarity*.

События должны публиковаться теми компонентами, в которых производятся соответствующие вычисления: в компоненте *RankCalculator* публикуется событие *RankCalculated*, соответственно *SimilarityCalculated* - в обработчике POST-запроса в компоненте "Web-приложение" (*Valuator*).

Сообщения событий должны содержать информацию, относящуюся к контексту события. Во-первых, это идентификатор контекста (эту роль в нашем приложении выполняет идентификатор текста в БД), а также другие данные, описывающие изменение состояния системы в результате наступления события. Для *RankCalculated* разумно передавать значение оценки *rank*, для *SimilarityCalculated* - значение *similarity*.

### Новый компонент

Необходимо добавить новый компонент *EventsLogger*, который подписывается на вышеуказанные события и выводит в консоль информацию о полученных сообщениях:
1. Название события
2. Идентификатор контекста
3. Значение *rank* для события *RankCalculated*
4. Значение *similarity* для события *SimilarityCalculated*

В скриптах необходимо добавить команды для запуска и останова двух экземпляров *EventsLogger*.

## Сылки
1. Описание шаблона "Издатель-Подписчик" https://docs.microsoft.com/en-us/azure/architecture/patterns/publisher-subscriber