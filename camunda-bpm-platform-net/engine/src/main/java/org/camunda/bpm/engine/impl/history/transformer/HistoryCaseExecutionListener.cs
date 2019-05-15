/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.history.transformer
{
	using CaseExecutionListener = org.camunda.bpm.engine.@delegate.CaseExecutionListener;
	using DelegateCaseExecution = org.camunda.bpm.engine.@delegate.DelegateCaseExecution;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using CmmnHistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.CmmnHistoryEventProducer;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class HistoryCaseExecutionListener : CaseExecutionListener
	{

	  protected internal CmmnHistoryEventProducer eventProducer;
	  protected internal HistoryLevel historyLevel;

	  public HistoryCaseExecutionListener(CmmnHistoryEventProducer historyEventProducer, HistoryLevel historyLevel)
	  {
		eventProducer = historyEventProducer;
		this.historyLevel = historyLevel;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateCaseExecution caseExecution) throws Exception
	  public virtual void notify(DelegateCaseExecution caseExecution)
	  {
		HistoryEvent historyEvent = createHistoryEvent(caseExecution);

		if (historyEvent != null)
		{
		  Context.ProcessEngineConfiguration.HistoryEventHandler.handleEvent(historyEvent);
		}

	  }

	  protected internal abstract HistoryEvent createHistoryEvent(DelegateCaseExecution caseExecution);

	}

}