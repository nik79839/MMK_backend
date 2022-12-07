﻿using Application.Interfaces;
using Domain.InitialResult;
using Domain.ProcessedResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ProcessResultService : IProcessResultService
    {
        /// <summary>
        /// Обработка результатов расчета
        /// </summary>
        /// <param name="powerFlowResults"></param>
        public StatisticBase Processing(List<CalculationResultBase> powerFlowResults)
        {
            List<double> values = powerFlowResults.ConvertAll(x => x.Value);
            StatisticBase statisticBase = new();
            statisticBase.GetStatistic(values);
            return statisticBase;
        }

        /// <summary>
        /// Обработка результатов расчета
        /// </summary>
        /// <param name="calculationResults"></param>
        public List<VoltageResultProcessed> Processing(List<VoltageResult> voltageResults)
        {
            List<VoltageResultProcessed> voltageResultProcessed = new();
            var nodeNumbers = voltageResults.Select(x => x.NodeNumber).ToArray().Distinct();
            foreach (var nodeNumber in nodeNumbers)
            {
                List<double> values = new();
                string nodeName = voltageResults.First(x => x.NodeNumber == nodeNumber).NodeName;
                values.AddRange(from voltageResult in voltageResults
                                where voltageResult.NodeNumber == nodeNumber
                                select voltageResult.Value);
                StatisticBase statisticBase = new VoltageResultProcessed();
                statisticBase.GetStatistic(values);
                VoltageResultProcessed statistic = (VoltageResultProcessed)statisticBase;
                statistic.NodeName = nodeName;
                voltageResultProcessed.Add(statistic);
            }
            return voltageResultProcessed;
        }

        public List<CurrentResultProcessed> Processing(List<CurrentResult> currentResults)
        {
            List<CurrentResultProcessed> currentResultProcesseds = new();
            var brunchNames = currentResults.Select(x => x.BrunchName).ToArray().Distinct();
            foreach (var brunchName in brunchNames)
            {
                List<double> values = new();
                values.AddRange(from currentResult in currentResults
                                where currentResult.BrunchName == brunchName
                                select currentResult.Value);
                StatisticBase statisticBase = new CurrentResultProcessed();
                statisticBase.GetStatistic(values);
                CurrentResultProcessed statistic = (CurrentResultProcessed)statisticBase;
                statistic.BrunchName = brunchName;
                currentResultProcesseds.Add(statistic);
            }
            return currentResultProcesseds;
        }
    }
}
