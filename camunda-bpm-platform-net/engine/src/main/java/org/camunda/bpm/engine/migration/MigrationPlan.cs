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
namespace org.camunda.bpm.engine.migration
{

	/// <summary>
	/// <para>Specifies how process instances from one process definition (the <i>source process definition</i>)
	/// should be migrated to another process definition (the <i>target process definition</i>).
	/// 
	/// </para>
	/// <para>A migration plan consists of a number of <seealso cref="MigrationInstruction"/>s that tell which
	///   activity maps to which. The set of instructions is complete, i.e. the migration logic does not perform
	///   migration steps that are not given by the instructions
	/// 
	/// @author Thorben Lindhauer
	/// </para>
	/// </summary>
	public interface MigrationPlan
	{

	  /// <returns> the list of instructions that this plan consists of </returns>
	  IList<MigrationInstruction> Instructions {get;}

	  /// <returns> the id of the process definition that is migrated from </returns>
	  string SourceProcessDefinitionId {get;}

	  /// <returns> the id of the process definition that is migrated to </returns>
	  string TargetProcessDefinitionId {get;}

	}

}