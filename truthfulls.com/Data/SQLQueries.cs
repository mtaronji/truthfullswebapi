﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using truthfulls.com.Models;

namespace truthfulls.com.Data
{
    //list of sql queries for sqllite
    public class SQLiteQueries
    {
        public static string Tickers()
        {
            return $@"select s.ticker from stock s";
        }
        public static string WeeklyPriceData(string ticker, string startDate, string endDate)
        {
            return $@"select t1.*, sp1.[open], sp2.[Close], sp2.[AdjClose] from
                    (
                    select min(p.[Date]) as weekstart, max(p.[date]) as weekend, max(p.[High]) as [High], min(p.[Low]) as [Low], sum(p.Volume) as Volume
                    from
                    (
                        select p.*, strftime('%Y', p.[date]) as yearnum, strftime('%W', p.[date]) as weeknum from[Price] p
                        where p.[Ticker] = '{ticker}'
                        ) p
                        group by p.yearnum, p.[weeknum]
                    ) t1
                    inner join
                    [Price] sp1 on
                    (sp1.[Date] = t1.weekstart and sp1.Ticker = '{ticker}')
                    inner join
                    [Price] sp2 on
                    (sp2.[Date] = t1.[weekend] and sp2.Ticker = '{ticker}')
                    where t1.weekend > DATE('{startDate}') and t1.weekend < DATE('{endDate}')";
        }

        public static string DailyPriceData(string ticker, string startDate, string endDate)
        {
            return $@"select t.[Date], t.[Low], t.[High], t.[Open], t.[Close], t.[Adjclose], t.[volume]
                    from[Price] t 
                    where t.[Ticker] = '{ticker}' and  t.[Date] > DATE('{startDate}') and t.[Date] < DATE('{endDate}')";
        }
        public static string DailyCloseData(string ticker, string startDate, string enddate)
        {
            return $@"select t.[Date], t.[Close] * 1.00
                    from[Price] t 
                    where t.[Ticker] = '{ticker}' and  t.[Date] > DATE('{startDate}') and t.[Date] < DATE('{enddate}') 
                    order by t.[date] asc";
        }
        public static string DailyGains(string ticker, string startDate, string endDate)
        {
            return $@"select t.* from
                    (select p.[Date], p.[close] * 1.0 /( lag(p.[Close], 1, 0) over(order by p.[date])*1.0)-1 as previous
                    from [price] p where p.Ticker = '{ticker}') t
                    where t.previous is not null
                    and t.[Date] > DATE('{startDate}') and t.[Date] < DATE('{endDate}') 
                    order by t.[date] asc";
        }

        public static string WeeklyGains(string ticker, int duration)
        {
            return

                    $@"
                     with weeklydata as 
                     (
                        select t1.*, sp1.[open], sp2.[Close] from
                        (
                            select min(p.[Date]) as weekstart, max(p.[date]) as weekend, max(p.[High]) as [High], min(p.[Low]) as [Low], sum(p.Volume) as Volume
                            from
                            (
                            select p.*, strftime('%Y', p.[date]) as yearnum,  strftime('%W', p.[date]) as weeknum from [Price] p
                            where p.[Ticker] = '{ticker}') p
                            group by p.yearnum, p.[weeknum]
                        ) t1
                        inner join
                        [Price] sp1 on
                        (sp1.[Date] = t1.[weekstart] and sp1.Ticker = '{ticker}')
                        inner join
                        [Price] sp2 on
                        (sp2.[Date] = t1.[weekend] and sp2.Ticker = '{ticker}')
                        where t1.weekend > DATE(DATE('NOW'), '{-duration} YEAR')
                    )

                    SELECT T.[weekstart], T.[weekend], T.Gain
                        from
                        (select wd.[weekstart], wd.[weekend], wd.[close] * 1.0 / (lag(wd.[close], 1, 0) over (order by wd.[weekstart]) * 1.0) - 1 as Gain 
                        from weeklydata wd) T
                    where T.Gain is not null";
        }

        //returns 
        public static string WeeklyConsecutivePGains(string ticker, int duration)
        {
            return

                $@"
                with weeklydata as 
                (
                    select t1.*, sp1.[open], sp2.[Close] from
                    (
                        select min(p.[Date]) as weekstart, max(p.[date]) as weekend, max(p.[High]) as [High], min(p.[Low]) as [Low], sum(p.Volume) as Volume
                        from
                        (
                        select p.*, strftime('%Y', p.[date]) as yearnum,  strftime('%W', p.[date]) as weeknum from [Price] p
                        where p.[Ticker] = '{ticker}') p
                        group by p.yearnum, p.[weeknum]
                    ) t1
                    inner join
                    [Price] sp1 on
                    (sp1.[Date] = t1.[weekstart] and sp1.Ticker = '{ticker}')
                    inner join
                    [Price] sp2 on
                    (sp2.[Date] = t1.[weekend] and sp2.Ticker = '{ticker}')
                    where t1.weekend > DATE('NOW', '-{duration} YEAR')
                ),

                gdata as (
                    select row_number() over (order by wd.[weekstart]) as gainrownum, wd.weekend,
                    wd.[close]/lag(wd.[close]) over (order by wd.weekstart) * 1.0 - 1 as Gain
                    from weeklydata wd
                ),
                PGains as (
                    select * from gdata gd
                    where gd.Gain > 0.0
                ),
                NGains as (
                    select * from gdata gd
                    where gd.Gain < 0.0
                ),
                ENDP as (
                    select t.* from (
                    select pg.gainrownum, lead(pg.[gainrownum]) over (order by pg.weekend) nxtrow, pg.weekend
                    from pgains pg) t
                    where t.nxtrow - t.gainrownum > 1
                )

                select min(ep.weekend) as gainenddate, ep.gainrownum - pg.gainrownum + 1 as gainendrow
                from PGAINS pg,ENDP ep
                where ep.weekend >= pg.weekend
                group by pg.weekend";

        }

        public static string DailyConsecutivePGains(string ticker, int duration)
        {
            return
            $@"
            with gdata as (
            select row_number() over (order by p.[date]) as gainrownum, p.[date],
            p.[close]/lag(p.[close]) over (order by p.[date]) * 1.0 - 1 as Gain
            from Price p
            where p.[ticker] = '{ticker}'
            and p.[date] > DATE('NOW', '-{duration} YEARS')
            ),
            PGains as (
                select * from gdata gd
                where gd.Gain > 0.0
            ),
            NGains as (
                select * from gdata gd
                where gd.Gain < 0.0
            ),
            ENDP as (
                select t.* from (
                select pg.gainrownum, lead(pg.[gainrownum]) over (order by pg.[date]) nxtrow, pg.[date]
                from pgains pg) t
                where t.nxtrow - t.gainrownum > 1
            )

            select pg.[date], ep.gainrownum - pg.gainrownum + 1 as gainendrow
            from PGAINS pg,ENDP ep
            where ep.[date] >= pg.[date]
            group by pg.[date]";
        }
        public static string SMA(string ticker, int duration)
        {
            return

            $@"
            select avg(n.[close]) as SMA from
            (
                select p.[close] from price p
                where p.[ticker] = '{ticker}'
                order by p.[date] desc
                limit {duration}
            ) n";
        }

        public static string EMA(string ticker, int duration)
        {
            return

            $@"
            select avg(n.[eclose]) as SMA from
            (
                select p.[close] * (2.0 / ({duration} + 1.0)) + p.[close] as eclose from price p 
                where p.[ticker] = '{ticker}'
                order by p.[date] desc
                limit {duration}
            ) n";
        }


    }
}
