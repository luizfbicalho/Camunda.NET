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
namespace org.camunda.bpm.engine.impl.persistence.entity
{


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class PropertyManager : AbstractManager
	{

	  public virtual PropertyEntity findPropertyById(string propertyId)
	  {
		return DbEntityManager.selectById(typeof(PropertyEntity), propertyId);
	  }

	  public virtual void acquireExclusiveLock()
	  {
		// We lock a special deployment lock property
		DbEntityManager.@lock("lockDeploymentLockProperty");

	  }

	  public virtual void acquireExclusiveLockForHistoryCleanupJob()
	  {
		// We lock a special history cleanup lock property
		DbEntityManager.@lock("lockHistoryCleanupJobLockProperty");

	  }

	  public virtual void acquireExclusiveLockForStartup()
	  {
		// We lock a special startup lock property
		DbEntityManager.@lock("lockStartupLockProperty");

	  }

	}

}