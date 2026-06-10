# README — Iteration 03 — Turn System, NPC, Win/Lose

## 1. Что было добавлено
Полноценный пошаговый цикл: Player Phase → World Phase (NPC двигаются по очереди детерминированно). Первый тип NPC — Linear swimmer (красный круг со стрелкой направления): плывёт по прямой, у препятствия разворачивается с анимацией стрелки. Победа при достижении выхода (радостные прыжки + звук), поражение при столкновении (вспышка, красный burst, screen shake, вибрация, анимация смерти). Попапы Win/Lose с backdrop и scale-анимациями, кнопка RESTART в HUD и в попапах — мгновенный полный rebuild уровня.

## 2. Что изменилось с прошлой итерации
**ЗАМЕНИТЬ файлы:**
- `Scripts/Gameplay/LevelData.cs` — новые символы раскладки `^ v < >` (Linear swimmer с направлением)
- `Scripts/Gameplay/GridManager.cs` — реестр занятости клеток NPC
- `Scripts/Gameplay/PlayerController.cs` — анимации победы и смерти, корректный сброс при рестарте
- `Scripts/Gameplay/LevelLoader.cs` — спавн NPC, делегирование ходов TurnManager'у, RestartLevel
- `Scripts/Gameplay/CameraController.cs` — добавлен Shake
- `Scripts/Gameplay/SpriteFactory.cs` — добавлен Triangle (стрелка NPC)
- `Scripts/Gameplay/SplashEffect.cs` — параметр intensity (мелкие всплески NPC) + фикс ParticleSystem из прошлого хотфикса
- `Scripts/UI/GameHUD.cs` — кнопка RESTART, свойство MovesCount

**НОВЫЕ файлы:** TurnManager, SwimmerBase, LinearSwimmer, CollisionEffect (Gameplay); PopupBase, WinPopup, LosePopup (UI); Iteration03Builder (Editor).

## 3. Editor скрипты — какие меню запустить и в каком порядке
1. `SwimGame → Update Test Levels (Iteration 3)` — перезапишет раскладки 3 тестовых уровней, добавив NPC (внимание: затрёт твои правки раскладок, если были)
2. `SwimGame → Update Game Scene (Iteration 3)` — дополнит сцену Game (НЕ пересоздаёт): TurnManager, SwimmersContainer, кнопка RESTART рядом с WAIT, попапы Win/Lose, все ссылки

## 4. Как тестировать
1. MainMenu → PLAY. На уровне красный пловец со стрелкой шныряет по коридору
2. Дойти до зелёного выхода, выбрав момент для пересечения коридора пловца — прыжки радости, звук победы, попап LEVEL COMPLETE с числом ходов
3. Заплыть в пловца (или дать ему врезаться в тебя, постояв на его пути через WAIT) — вспышка, тряска камеры, вибрация (на устройстве), попап OUCH!
4. RESTART из HUD и из попапов — уровень мгновенно пересобирается, счётчик ходов сбрасывается
5. Проверить TestLevel2 (два NPC, вертикальный и горизонтальный) и TestLevel3 (три NPC + скролл)

## 5. Ожидаемый результат
NPC двигаются СТРОГО после хода игрока (включая WAIT), по одному с микро-задержкой 0.05s — World Phase читается глазами. Стрелка NPC всегда показывает, куда он пойдёт следующим ходом, при развороте стрелка плавно перелетает на другую сторону. Столкновение в любую сторону (ты в него / он в тебя / обмен клетками) = поражение. Попапы появляются scale 0→1 с Ease.OutBack поверх затемнения.

## 6. Известные ограничения текущей итерации
- Только Linear swimmer (остальные 4 типа AI — итерация 4)
- В Win попапе нет звёзд и кнопки Next Level (система уровней — итерация 7)
- После победы/поражения World Phase останавливается мгновенно — недоигравшие NPC замирают (намеренно, для читаемости смерти)

## 7. Что будет в следующей итерации
Все типы AI (Patrol, Fast, Delayed, Reactive), визуальное различие типов, полировка эффектов движения.
