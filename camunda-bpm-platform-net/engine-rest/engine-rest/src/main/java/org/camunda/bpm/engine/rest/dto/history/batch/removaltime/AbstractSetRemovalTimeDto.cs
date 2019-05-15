using System;

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
	public abstract class AbstractSetRemovalTimeDto
	{

	  protected internal DateTime absoluteRemovalTime;
	  protected internal bool clearedRemovalTime;
	  protected internal bool calculatedRemovalTime;

	  public virtual DateTime AbsoluteRemovalTime
	  {
		  get
		  {
			return absoluteRemovalTime;
		  }
	  }

	  public virtual bool ClearedRemovalTime
	  {
		  get
		  {
			return clearedRemovalTime;
		  }
		  set
		  {
			this.clearedRemovalTime = value;
		  }
	  }


	  public virtual bool CalculatedRemovalTime
	  {
		  get
		  {
			return calculatedRemovalTime;
		  }
		  set
		  {
			this.calculatedRemovalTime = value;
		  }
	  }


	}

}