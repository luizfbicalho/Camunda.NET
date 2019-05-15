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
namespace org.camunda.bpm.engine.rest.dto.history.optimize
{
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;

	public class HistoricOptimizeVariableUpdateDto : HistoricVariableUpdateDto
	{

	  protected internal long sequenceCounter;

	  public virtual long SequenceCounter
	  {
		  get
		  {
			return sequenceCounter;
		  }
		  set
		  {
			this.sequenceCounter = value;
		  }
	  }


	  public static HistoricOptimizeVariableUpdateDto fromHistoricVariableUpdate(HistoricVariableUpdate historicVariableUpdate)
	  {
		HistoricOptimizeVariableUpdateDto dto = new HistoricOptimizeVariableUpdateDto();
		fromHistoricVariableUpdate(dto, historicVariableUpdate);
		fromHistoricDetail(historicVariableUpdate, dto);
		if (historicVariableUpdate is HistoryEvent)
		{
		  HistoryEvent historyEvent = (HistoryEvent) historicVariableUpdate;
		  dto.SequenceCounter = historyEvent.SequenceCounter;
		}
		return dto;
	  }

	}

}