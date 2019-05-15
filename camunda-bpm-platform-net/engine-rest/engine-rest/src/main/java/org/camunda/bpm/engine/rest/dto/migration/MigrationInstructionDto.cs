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
namespace org.camunda.bpm.engine.rest.dto.migration
{

	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;

	public class MigrationInstructionDto
	{

	  protected internal IList<string> sourceActivityIds;
	  protected internal IList<string> targetActivityIds;
	  protected internal bool? updateEventTrigger;

	  public virtual IList<string> SourceActivityIds
	  {
		  get
		  {
			return sourceActivityIds;
		  }
		  set
		  {
			this.sourceActivityIds = value;
		  }
	  }


	  public virtual IList<string> TargetActivityIds
	  {
		  get
		  {
			return targetActivityIds;
		  }
		  set
		  {
			this.targetActivityIds = value;
		  }
	  }


	  public virtual bool? UpdateEventTrigger
	  {
		  set
		  {
			this.updateEventTrigger = value;
		  }
		  get
		  {
			return updateEventTrigger;
		  }
	  }


	  public static MigrationInstructionDto from(MigrationInstruction migrationInstruction)
	  {
		if (migrationInstruction != null)
		{
		  MigrationInstructionDto dto = new MigrationInstructionDto();

		  dto.SourceActivityIds = Collections.singletonList(migrationInstruction.SourceActivityId);
		  dto.TargetActivityIds = Collections.singletonList(migrationInstruction.TargetActivityId);
		  dto.UpdateEventTrigger = migrationInstruction.UpdateEventTrigger;

		  return dto;
		}
		else
		{
		  return null;
		}
	  }

	}

}