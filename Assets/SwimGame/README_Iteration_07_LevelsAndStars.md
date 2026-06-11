# README — Iteration 07 — 30 Levels, Level Select, Stars & Progress

## 1. Что было добавлено
- **30 уровней кампании** (Level_01..Level_30) с прогрессией сложности: чистое движение (1-2), Linear (3-5), Patrol (6-7), Delayed (8), Fast (9-10), Reactive (11-12), течения (13-15), лодки (16-17), платформы (18-20), комбинации (21-29), финальный гаунтлет (30). КАЖДЫЙ уровень проверен BFS-солвером, который точно повторяет игровую логику: 100% проходимы, оптимум посчитан машинно
- **LevelDatabase** — ScriptableObject со списком уровней по порядку
- **Экран выбора уровней** (сцена LevelSelect): сетка 5x6, номер + 3 звезды на ячейке, замки на непройденных (открывается следующий после любого прохождения предыдущего), PLAY в главном меню теперь ведёт сюда
- **Звёзды**: ходы <= оптимум — 3 звезды, <= оптимум+3 — 2, иначе 1. Лучший результат сохраняется (PlayerPrefs)
- **WinPopup переделан**: 3 звезды с poppy-анимацией (OutBack со стаггером + звук за каждую), "Moves: X  Best: Y", кнопка NEXT (скрыта на 30-м уровне), MENU теперь ведёт в выбор уровней

## 2. Что изменилось с прошлой итерации
**ЗАМЕНИТЬ файлы:**
- `Scripts/Gameplay/LevelData.cs` — поле optimalMoves
- `Scripts/Gameplay/LevelLoader.cs` — загрузка по индексу из базы, статический SelectedLevelIndex, сохранение звёзд, NextLevel()
- `Scripts/UI/WinPopup.cs` — ShowResult со звёздами и NEXT
- `Scripts/UI/LosePopup.cs` — MENU ведёт в LevelSelect
- `Scripts/UI/MainMenuUI.cs` — PLAY ведёт в LevelSelect

**НОВЫЕ файлы:** LevelDatabase.cs, ProgressManager.cs (Gameplay); LevelSelectUI.cs (UI); Iteration07Builder.cs (Editor).

## 3. Editor скрипты — какие меню запустить и в каком порядке
1. `SwimGame -> Create 30 Levels + Database (Iteration 7)` — создаст Assets/SwimGame/Levels/Campaign/Level_01..30 и LevelDatabase. ВНИМАНИЕ: повторный запуск перезаписывает раскладки уровней (это источник истины)
2. `SwimGame -> Build Level Select Scene (Iteration 7)` — создаст сцену LevelSelect и добавит в Build Settings
3. `SwimGame -> Update Game Scene (Iteration 7)` — прокинет базу в LevelLoader и пересоберёт WinPopup (старый удаляется)

## 4. Как тестировать
1. MainMenu -> PLAY -> экран LEVELS: открыт только уровень 1, остальные тусклые с замком
2. Пройти уровень 1 за 7 ходов -> 3 звезды с анимацией -> NEXT грузит уровень 2 без выхода в меню
3. Пройти за 8-10 ходов -> 2 звезды; за 11+ -> 1 звезда. Перепройти лучше — звёзды обновятся, хуже — останется лучший результат
4. Вернуться в LEVELS: на пройденных ячейках видны звёзды, следующий уровень разблокирован
5. Удалить PlayerPrefs (Edit -> Clear All PlayerPrefs) — прогресс сброшен, снова открыт только уровень 1
6. Запуск сцены Game напрямую из редактора без выбора уровня — грузится Default Level как раньше (SelectedLevelIndex = -1)

## 5. Ожидаемый результат
Полный игровой цикл: меню -> выбор уровня -> игра -> звёзды -> следующий уровень. 30 уровней, в которых препятствия реально мешают (солвер-решения содержат ожидания, отступления и обходы — см. walkthrough ниже).

## 6. Известные ограничения текущей итерации
- Звёзды — кружки-плейсхолдеры (заменятся ассетами)
- Монет за прохождение пока нет (итерация 8)
- Сетка уровней без скролла — рассчитана ровно на 30; при добавлении уровней понадобится ScrollRect

## 7. Что будет в следующей итерации
Экономика: монеты за прохождение (+бонус за звёзды), анимация прилёта монет, валюта в top bar, магазин предметов вместо дебаг-выдачи x3.

## 8. Walkthrough (решения солвера, U=вверх D=вниз L=влево R=вправо W=wait)
Это машинно-проверенные оптимальные решения. Звёзды: 3 = уложиться в оптимум, 2 = оптимум+3.

**Level 1** — оптимум 7: `D D D D R R R`
**Level 2** — оптимум 14: `R R R D D L L L D D D R R R`
**Level 3** — оптимум 10: `R R R D D L D D D R`
**Level 4** — оптимум 23: `R R R R D D L L D D U D R R D W D L L D D R R`
**Level 5** — оптимум 20: `R L R L R L R L R R D D D D L W R D D D`
**Level 6** — оптимум 8: `D D D D R R R R`
**Level 7** — оптимум 14: `D D D R R R R D D L D D R R`
**Level 8** — оптимум 22: `R L R L R L R R R R D D L L D W W D R R D D`
**Level 9** — оптимум 14: `D R D D R R R D D L D D R R`
**Level 10** — оптимум 14: `D R R R D D R D D L D D R R`
**Level 11** — оптимум 11: `D R R R R D D D D D R`
**Level 12** — оптимум 16: `D R D D W R W R D D R D D D D R`
**Level 13** — оптимум 6: `R R D D R R`
**Level 14** — оптимум 9: `D D D D R R R R R`
**Level 15** — оптимум 16: `R L L L L L R D D L D D R D D R`
**Level 16** — оптимум 14: `R R R R D W D L D D R D D R`
**Level 17** — оптимум 14: `R R R R D D D D L L D D R R`
**Level 18** — оптимум 8: `R R R R D D D D`
**Level 19** — оптимум 34: `R R R R R D D L L L L L D D R R R R R W D D L L L L L D D R R R R R`
**Level 20** — оптимум 14: `R R R R D W D L D D R D D R`
**Level 21** — оптимум 18: `D D D R R R R R D D W L L D D R R R`
**Level 22** — оптимум 16: `R L R L R R R R D D L D D D D R`
**Level 23** — оптимум 22: `R R R R D D L D D R D U D D L L D W D R R R`
**Level 24** — оптимум 18: `D D D R R R R R D D W L L D D R R R`
**Level 25** — оптимум 17: `D D D R R R R R R D D L L D D R D`
**Level 26** — оптимум 32: `R L R L R L R R R R R R D W D L L D D R R D D L L L D D R R R R`
**Level 27** — оптимум 18: `D D D R R R R R R D D L D D R R R R`
**Level 28** — оптимум 19: `D D D R R R R R R D D L L W D D R R R`
**Level 29** — оптимум 25: `D U D U D D D D R R R R R R D D L L L D D R R R R`
**Level 30** — оптимум 28: `D D R R R R R R D D L L D D L W D D L L L W D D R R R R`
