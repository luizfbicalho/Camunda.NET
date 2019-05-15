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
namespace org.camunda.bpm.engine.rest.dto.history.batch.removaltime
{

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class SetRemovalTimeToHistoricDecisionInstancesDto : AbstractSetRemovalTimeDto
	{

	  protected internal string[] historicDecisionInstanceIds;
	  protected internal HistoricDecisionInstanceQueryDto historicDecisionInstanceQuery;
	  protected internal bool hierarchical;

	  public virtual string[] HistoricDecisionInstanceIds
	  {
		  get
		  {
			return historicDecisionInstanceIds;
		  }
		  set
		  {
			this.historicDecisionInstanceIds = value;
		  }
	  }


	  public virtual HistoricDecisionInstanceQueryDto HistoricDecisionInstanceQuery
	  {
		  get
		  {
			return historicDecisionInstanceQuery;
		  }
		  set
		  {
			this.historicDecisionInstanceQuery = value;
		  }
	  }


	  public virtual bool Hierarchical
	  {
		  get
		  {
			return hierarchical;
		  }
		  set
		  {
			this.hierarchical = value;
		  }
	  }


	}

}