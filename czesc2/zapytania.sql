-- a) liczba temat�w utworzonych w 2013 roku   149
--SELECT COUNT(*)
--FROM [dbo].[Threads] t
--WHERE THR_CreationDate BETWEEN '2013-01-01' AND '2013-12-31'

-- b) najbardziej popularny temat w maju 2013    Post-Mortem XV Tolk Folku i Pomys�y na XVI
--SELECT TOP 1 THR_Title
--FROM [dbo].[Threads] t
--	JOIN [dbo].[Posts] p
--	 ON t.THR_ID = p.PST_THRID
--WHERE p.PST_CreationDate BETWEEN '2013-01-01' AND '2013-12-31'
--GROUP BY THR_Title
--ORDER BY COUNT(*) DESC

-- c) �rednia d�ugo�� tekstu posta    446
--SELECT AVG(LEN(p.PST_Content))
--FROM [dbo].[Posts] p	

-- d)  u�ytkownik wypowiadaj�cy si� w najwi�kszej liczbie temat�w   M.L.
--SELECT TOP 1 u.USR_Login
--FROM [dbo].[Users] u
--	JOIN (
--			SELECT p.PST_USRID AS USR
--			FROM [dbo].[Posts] p
--			GROUP BY p.PST_USRID, p.PST_THRID
--		 ) a
--		ON u.USR_ID = a.USR
--GROUP BY a.USR, u.USR_Login
--ORDER BY COUNT(*) DESC
-- lub
--SELECT TOP 1 u.USR_Login
--FROM dbo.Users u, dbo.Threads t
--WHERE EXISTS ( SELECT * FROM Posts p WHERE p.PST_USRID = u.USR_ID AND p.PST_THRID = t.THR_ID)
--GROUP BY u.USR_Login
--ORDER BY COUNT(*) DESC

-- e)  u�ytkownik komentuj�cy najwi�ksz� liczb� innych u�ytkownik�w     Sirielle
--SELECT TOP 1 u.USR_Login
--FROM
--(
--	SELECT p.PST_USRID AS USR
--	FROM [dbo].[Posts] p
--		JOIN [dbo].[Threads] t
--			ON p.PST_THRID = t.THR_ID 
--	WHERE T.THR_USRID != PST_USRID
--	GROUP BY p.PST_USRID, t.THR_ID
--) a
--	JOIN [dbo].[Users] u
--		ON a.USR = u.USR_ID
--GROUP BY u.USR_Login
--ORDER BY COUNT(*) DESC
--lub
--SELECT TOP 1 u.USR_Login
--FROM dbo.Users u, dbo.Threads t
--WHERE EXISTS ( SELECT * FROM Posts p WHERE p.PST_USRID = u.USR_ID AND p.PST_THRID = t.THR_ID AND t.THR_USRID != u.USR_ID)
--GROUP BY u.USR_Login
--ORDER BY COUNT(*) DESC

-- f) liczba post�w zawieraj�cych s�owo 'Frodo'     158
--SELECT COUNT(*)
--FROM [dbo].[Posts] p
--WHERE p.PST_Content like '%Frodo%'
-- lub
--SELECT COUNT(*)
--FROM [dbo].[Posts] p
--WHERE CONTAINS(p.PST_Content, 'Frodo')

-- g) liczba post�w wys�anych przez u�ytkownik�w z miasta na liter� 'K'     631
--SELECT COUNT(*)
--FROM [dbo].[Users] u
--	JOIN [dbo].[Posts] p
--		ON u.USR_ID = p.PST_USRID
--WHERE u.USR_City like 'K%'

-- h) 35te najcz�ciej u�yte s�owo w tre�ci posta
--SELECT display_term FROM sys.dm_fts_index_keywords(db_id('TolkienForum'), object_id('dbo.Posts'))
--ORDER BY document_count DESC
--OFFSET 35 ROWS
--FETCH NEXT 1 ROWS ONLY

