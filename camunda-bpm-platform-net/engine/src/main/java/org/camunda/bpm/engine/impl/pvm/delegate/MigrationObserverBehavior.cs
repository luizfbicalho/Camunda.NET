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
namespace org.camunda.bpm.engine.impl.pvm.@delegate
{
	using MigratingActivityInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingActivityInstance;
	using MigratingInstanceParseContext = org.camunda.bpm.engine.impl.migration.instance.parser.MigratingInstanceParseContext;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public interface MigrationObserverBehavior
	{

	  /// <summary>
	  /// Implement to perform activity-specific migration behavior that is not
	  /// covered by the regular migration procedure. Called after the scope execution and any ancestor executions
	  /// have been migrated to their target activities and process definition.
	  /// </summary>
	  void migrateScope(ActivityExecution scopeExecution);

	  /// <summary>
	  /// Callback to implement behavior specific parsing (e.g. adding additional dependent entities).
	  /// </summary>
	  void onParseMigratingInstance(MigratingInstanceParseContext parseContext, MigratingActivityInstance migratingInstance);
	}

}