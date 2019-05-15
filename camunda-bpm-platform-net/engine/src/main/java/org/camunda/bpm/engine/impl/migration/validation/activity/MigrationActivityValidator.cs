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
namespace org.camunda.bpm.engine.impl.migration.validation.activity
{
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;

	/// <summary>
	/// Check if an activity can be migrated. For example
	/// if the activity type is supported by the migration.
	/// </summary>
	public interface MigrationActivityValidator
	{

	  /// <summary>
	  /// Check that an activity can be migrated.
	  /// </summary>
	  /// <param name="activity"> the activity to migrate </param>
	  /// <returns> true if the activity can be migrated, false otherwise </returns>
	  bool valid(ActivityImpl activity);

	}

}