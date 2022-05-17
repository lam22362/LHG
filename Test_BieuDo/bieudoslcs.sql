declare @month int, @year int
                            IF Object_Id('Tempdb..#TABLE_DAY') IS NOT NULL DROP TABLE #TABLE_DAY
                            CREATE TABLE #TABLE_DAY(dDAY DATE)
                            set @month = '6'
                            set @year = '2017'
                            /*Select cac ngay trong thang*/
                            INSERT INTO #TABLE_DAY
                            SELECT	CONVERT(VARCHAR, CAST(CAST(@year AS VARCHAR) + '-' + CAST(@Month AS VARCHAR) + '-01' AS DATETIME) + Number, 101) DDAY
                            FROM	master..spt_values
                            WHERE	type = 'P' AND (CAST(CAST(@year AS VARCHAR) + '-' + CAST(@Month AS VARCHAR) + '-01' AS DATETIME) + Number ) < DATEADD(mm,1,CAST(CAST(@year AS VARCHAR) + '-' + CAST(@Month AS VARCHAR) + '-01' AS DATETIME))

                           SELECT	dDAY, sanluong num
                            FROM	sanluongcausu U RIGHT JOIN #TABLE_DAY D ON CONVERT(DATE, ngaylaysolieu) = dDAY AND khudat = 'Khu A'
                            