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
namespace org.camunda.bpm.engine.impl.migration
{
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationInstructionImpl : MigrationInstruction
	{

	  protected internal string sourceActivityId;
	  protected internal string targetActivityId;

	  protected internal bool updateEventTrigger = false;

	  public MigrationInstructionImpl(string sourceActivityId, string targetActivityId) : this(sourceActivityId, targetActivityId, false)
	  {
	  }

	  public MigrationInstructionImpl(string sourceActivityId, string targetActivityId, bool updateEventTrigger)
	  {
		this.sourceActivityId = sourceActivityId;
		this.targetActivityId = targetActivityId;
		this.updateEventTrigger = updateEventTrigger;
	  }

	  public virtual string SourceActivityId
	  {
		  get
		  {
			return sourceActivityId;
		  }
	  }

	  public virtual string TargetActivityId
	  {
		  get
		  {
			return targetActivityId;
		  }
	  }

	  public virtual bool UpdateEventTrigger
	  {
		  get
		  {
			return updateEventTrigger;
		  }
		  set
		  {
			this.updateEventTrigger = value;
		  }
	  }


	  public override string ToString()
	  {
		return "MigrationInstructionImpl{" +
		  "sourceActivityId='" + sourceActivityId + '\'' +
		  ", targetActivityId='" + targetActivityId + '\'' +
		  ", updateEventTrigger='" + updateEventTrigger + '\'' +
		  '}';
	  }

	}

}