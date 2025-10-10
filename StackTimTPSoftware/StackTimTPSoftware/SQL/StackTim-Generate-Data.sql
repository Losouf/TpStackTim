USE StacktimDb;
GO

INSERT INTO Players (Pseudo, Email, Rank, TotalScore) VALUES
('Astra',  'astra@stacktim.gg',  'Or',       820),
('Blitz',  'blitz@stacktim.gg',  'Platine',  1240),
('Cypher', 'cypher@stacktim.gg', 'Argent',   540),
('Drake',  'drake@stacktim.gg',  'Bronze',   310),
('Echo',   'echo@stacktim.gg',   'Fer',      120),
('Frost',  'frost@stacktim.gg',  'Diamant',  1710),
('Ghost',  'ghost@stacktim.gg',  'Or',       860),
('Haze',   'haze@stacktim.gg',   'Platine',  1330),
('Iris',   'iris@stacktim.gg',   'Argent',   620),
('Jinx',   'jinx@stacktim.gg',   'Or',       905),
('Kairo',  'kairo@stacktim.gg',  'Bronze',   275),
('Luna',   'luna@stacktim.gg',   'Diamant',  1820);
GO

INSERT INTO Teams (Name, Tag) VALUES
('StackTeam', 'STM'),
('NovaCore',  'NVA'),
('Raptors',   'RAP');
GO

-- TeamPlayers (0=Capitaine, 1=Membre, 2=Remplaçant)
INSERT INTO TeamPlayers (TeamId, PlayerId, Role) VALUES
((SELECT Id FROM Teams WHERE Tag='STM'), (SELECT Id FROM Players WHERE Pseudo='Blitz'),  0),
((SELECT Id FROM Teams WHERE Tag='STM'), (SELECT Id FROM Players WHERE Pseudo='Astra'),  1),
((SELECT Id FROM Teams WHERE Tag='STM'), (SELECT Id FROM Players WHERE Pseudo='Cypher'), 1),
((SELECT Id FROM Teams WHERE Tag='STM'), (SELECT Id FROM Players WHERE Pseudo='Echo'),   2);

INSERT INTO TeamPlayers (TeamId, PlayerId, Role) VALUES
((SELECT Id FROM Teams WHERE Tag='NVA'), (SEL	ECT Id FROM Players WHERE Pseudo='Frost'),  0),
((SELECT Id FROM Teams WHERE Tag='NVA'), (SELECT Id FROM Players WHERE Pseudo='Haze'),   1),
((SELECT Id FROM Teams WHERE Tag='NVA'), (SELECT Id FROM Players WHERE Pseudo='Iris'),   1),
((SELECT Id FROM Teams WHERE Tag='NVA'), (SELECT Id FROM Players WHERE Pseudo='Kairo'),  2);

INSERT INTO TeamPlayers (TeamId, PlayerId, Role) VALUES
((SELECT Id FROM Teams WHERE Tag='RAP'), (SELECT Id FROM Players WHERE Pseudo='Luna'),   0),
((SELECT Id FROM Teams WHERE Tag='RAP'), (SELECT Id FROM Players WHERE Pseudo='Jinx'),   1),
((SELECT Id FROM Teams WHERE Tag='RAP'), (SELECT Id FROM Players WHERE Pseudo='Ghost'),  1),
((SELECT Id FROM Teams WHERE Tag='RAP'), (SELECT Id FROM Players WHERE Pseudo='Drake'),  2);
GO
