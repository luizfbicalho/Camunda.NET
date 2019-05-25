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
namespace org.camunda.bpm.engine.rest.helper
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;

	public class MockMigrationInstructionBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal IList<string> sourceActivityIds_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal IList<string> targetActivityIds_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool updateEventTrigger_Conflict = false;

	  public virtual MockMigrationInstructionBuilder sourceActivityIds(IList<string> sourceActivityIds)
	  {
		this.sourceActivityIds_Conflict = sourceActivityIds;
		return this;
	  }

	  public virtual MockMigrationInstructionBuilder sourceActivityId(string sourceActivityId)
	  {
		this.sourceActivityIds_Conflict = Collections.singletonList(sourceActivityId);
		return this;
	  }

	  public virtual MockMigrationInstructionBuilder targetActivityIds(IList<string> targetActivityIds)
	  {
		this.targetActivityIds_Conflict = targetActivityIds;
		return this;
	  }

	  public virtual MockMigrationInstructionBuilder targetActivityId(string targetActivityId)
	  {
		this.targetActivityIds_Conflict = Collections.singletonList(targetActivityId);
		return this;
	  }

	  public virtual MockMigrationInstructionBuilder updateEventTrigger()
	  {
		this.updateEventTrigger_Conflict = true;
		return this;
	  }

	  public virtual MigrationInstruction build()
	  {
		MigrationInstruction migrationInstructionMock = mock(typeof(MigrationInstruction));
		when(migrationInstructionMock.SourceActivityId).thenReturn(sourceActivityIds_Conflict[0]);
		when(migrationInstructionMock.TargetActivityId).thenReturn(targetActivityIds_Conflict[0]);
		when(migrationInstructionMock.UpdateEventTrigger).thenReturn(updateEventTrigger_Conflict);
		return migrationInstructionMock;
	  }

	}

}