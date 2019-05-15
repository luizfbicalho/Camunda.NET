using System.Collections.Generic;

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
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationPlanImpl : MigrationPlan
	{

	  protected internal string sourceProcessDefinitionId;
	  protected internal string targetProcessDefinitionId;

	  protected internal IList<MigrationInstruction> instructions;

	  public MigrationPlanImpl(string sourceProcessDefinitionId, string targetProcessDefinitionId)
	  {
		this.sourceProcessDefinitionId = sourceProcessDefinitionId;
		this.targetProcessDefinitionId = targetProcessDefinitionId;
		this.instructions = new List<MigrationInstruction>();
	  }

	  public virtual string SourceProcessDefinitionId
	  {
		  get
		  {
			return sourceProcessDefinitionId;
		  }
		  set
		  {
			this.sourceProcessDefinitionId = value;
		  }
	  }


	  public virtual string TargetProcessDefinitionId
	  {
		  get
		  {
			return targetProcessDefinitionId;
		  }
		  set
		  {
			this.targetProcessDefinitionId = value;
		  }
	  }


	  public virtual IList<MigrationInstruction> Instructions
	  {
		  get
		  {
			return instructions;
		  }
		  set
		  {
			this.instructions = value;
		  }
	  }


	  public override string ToString()
	  {
		return "MigrationPlan[" +
		  "sourceProcessDefinitionId='" + sourceProcessDefinitionId + '\'' +
		  ", targetProcessDefinitionId='" + targetProcessDefinitionId + '\'' +
		  ", instructions=" + instructions +
		  ']';
	  }

	}

}