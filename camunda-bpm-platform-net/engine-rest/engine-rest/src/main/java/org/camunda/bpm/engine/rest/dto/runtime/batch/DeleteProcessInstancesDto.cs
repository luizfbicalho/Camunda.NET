﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.rest.dto.runtime.batch
{
	using HistoricProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceQueryDto;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	public class DeleteProcessInstancesDto
	{
	  protected internal IList<string> processInstanceIds;
	  protected internal ProcessInstanceQueryDto processInstanceQuery;
	  protected internal string deleteReason;
	  protected internal bool skipCustomListeners;
	  protected internal HistoricProcessInstanceQueryDto historicProcessInstanceQuery;
	  protected internal bool skipSubprocesses;

	  public virtual IList<string> ProcessInstanceIds
	  {
		  get
		  {
			return processInstanceIds;
		  }
		  set
		  {
			this.processInstanceIds = value;
		  }
	  }


	  public virtual ProcessInstanceQueryDto ProcessInstanceQuery
	  {
		  get
		  {
			return processInstanceQuery;
		  }
		  set
		  {
			this.processInstanceQuery = value;
		  }
	  }


	  public virtual string DeleteReason
	  {
		  get
		  {
			return deleteReason;
		  }
		  set
		  {
			this.deleteReason = value;
		  }
	  }


	  public virtual bool SkipCustomListeners
	  {
		  get
		  {
			return skipCustomListeners;
		  }
		  set
		  {
			this.skipCustomListeners = value;
		  }
	  }


	  public virtual HistoricProcessInstanceQueryDto HistoricProcessInstanceQuery
	  {
		  set
		  {
			this.historicProcessInstanceQuery = value;
		  }
		  get
		  {
			return historicProcessInstanceQuery;
		  }
	  }


	  public virtual bool isSkipSubprocesses()
	  {
		return skipSubprocesses;
	  }

	  public virtual void setSkipSubprocesses(bool? skipSubprocesses)
	  {
		this.skipSubprocesses = skipSubprocesses.Value;
	  }

	}

}