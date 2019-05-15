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
namespace org.camunda.bpm.engine.impl.migration.instance
{

	using TreeVisitor = org.camunda.bpm.engine.impl.tree.TreeVisitor;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class DeleteUnmappedInstanceVisitor : TreeVisitor<MigratingScopeInstance>
	{

	  protected internal ISet<MigratingScopeInstance> visitedInstances = new HashSet<MigratingScopeInstance>();

	  protected internal bool skipCustomListeners;
	  protected internal bool skipIoMappings;

	  public DeleteUnmappedInstanceVisitor(bool skipCustomListeners, bool skipIoMappings)
	  {
		this.skipCustomListeners = skipCustomListeners;
		this.skipIoMappings = skipIoMappings;
	  }

	  public virtual void visit(MigratingScopeInstance currentInstance)
	  {

		visitedInstances.Add(currentInstance);
		if (!currentInstance.migrates())
		{
		  ISet<MigratingProcessElementInstance> children = new HashSet<MigratingProcessElementInstance>(currentInstance.Children);
		  MigratingScopeInstance parent = currentInstance.Parent;

		  // 1. detach children
		  currentInstance.detachChildren();

		  // 2. manipulate execution tree (i.e. remove this instance)
		  currentInstance.remove(skipCustomListeners, skipIoMappings);

		  // 3. reconnect parent and children
		  foreach (MigratingProcessElementInstance child in children)
		  {
			child.attachState(parent);
		  }
		}
		else
		{
		  currentInstance.removeUnmappedDependentInstances();
		}
	  }

	  public virtual bool hasVisitedAll(ICollection<MigratingScopeInstance> activityInstances)
	  {
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
		return visitedInstances.containsAll(activityInstances);
	  }
	}

}