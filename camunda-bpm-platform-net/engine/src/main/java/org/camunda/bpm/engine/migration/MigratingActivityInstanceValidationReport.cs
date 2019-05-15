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
namespace org.camunda.bpm.engine.migration
{

	/// <summary>
	/// Collects all failures for a migrating activity instance.
	/// </summary>
	public interface MigratingActivityInstanceValidationReport
	{

	  /// <returns> the id of the source scope of the migrated activity instance </returns>
	  string SourceScopeId {get;}

	  /// <returns> the activity instance id of this report </returns>
	  string ActivityInstanceId {get;}

	  /// <returns> the migration instruction that cannot be applied </returns>
	  MigrationInstruction MigrationInstruction {get;}

	  /// <returns> true if the reports contains failures, false otherwise </returns>
	  bool hasFailures();

	  /// <returns> the list of failures </returns>
	  IList<string> Failures {get;}

	}

}