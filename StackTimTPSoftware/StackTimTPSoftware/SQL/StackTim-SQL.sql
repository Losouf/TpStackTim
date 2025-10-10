SELECT Pseudo, TotalScore
FROM Players
ORDER BY TotalScore DESC;


SELECT T.Name AS TeamName, P.Pseudo, TP.Role
FROM TeamPlayers TP
JOIN Players P ON P.Id = TP.PlayerId
JOIN Teams T ON T.Id = TP.TeamId
WHERE T.Id = 1;


SELECT T.Name AS TeamName,
       COUNT(P.Id) AS NbJoueurs,
       AVG(P.TotalScore) AS ScoreMoyen
FROM Teams T
LEFT JOIN TeamPlayers TP ON T.Id = TP.TeamId
LEFT JOIN Players P ON TP.PlayerId = P.Id
GROUP BY T.Name;


SELECT P.Id, P.Pseudo
FROM Players P
LEFT JOIN TeamPlayers TP ON P.Id = TP.PlayerId
WHERE TP.PlayerId IS NULL;


SELECT Rank, COUNT(*) AS NbJoueurs
FROM Players
GROUP BY Rank;