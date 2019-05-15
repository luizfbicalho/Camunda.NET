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
namespace org.camunda.bpm.engine.impl.pvm.runtime.operation
{


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class PvmAtomicOperationDeleteCascade : PvmAtomicOperation
	{

	  public virtual bool isAsync(PvmExecutionImpl execution)
	  {
		return false;
	  }

	  public virtual bool AsyncCapable
	  {
		  get
		  {
			return false;
		  }
	  }

	  public virtual void execute(PvmExecutionImpl execution)
	  {
		PvmExecutionImpl nextLeaf;
		do
		{
		  nextLeaf = findNextLeaf(execution);

		  // propagate skipCustomListeners property
		  PvmExecutionImpl deleteRoot = getDeleteRoot(execution);
		  if (deleteRoot != null)
		  {
			nextLeaf.SkipCustomListeners = deleteRoot.SkipCustomListeners;
			nextLeaf.SkipIoMappings = deleteRoot.SkipIoMappings;
		  }

		  PvmExecutionImpl subProcessInstance = nextLeaf.SubProcessInstance;
		  if (subProcessInstance != null)
		  {
			if (deleteRoot.SkipSubprocesses)
			{
			  subProcessInstance.SuperExecution = null;
			}
			else
			{
			  subProcessInstance.deleteCascade(execution.DeleteReason, nextLeaf.SkipCustomListeners, nextLeaf.SkipIoMappings);
			}
		  }

		  nextLeaf.performOperation(PvmAtomicOperation_Fields.DELETE_CASCADE_FIRE_ACTIVITY_END);

		} while (!nextLeaf.DeleteRoot);

	  }

	  protected internal virtual PvmExecutionImpl findNextLeaf(PvmExecutionImpl execution)
	  {
		if (execution.hasChildren())
		{
		  return findNextLeaf(execution.Executions[0]);
		}
		return execution;
	  }

	  protected internal virtual PvmExecutionImpl getDeleteRoot(PvmExecutionImpl execution)
	  {
		if (execution == null)
		{
		  return null;
		}
		else if (execution.DeleteRoot)
		{
		  return execution;
		}
		else
		{
		  return getDeleteRoot(execution.Parent);
		}
	  }

	  public virtual string CanonicalName
	  {
		  get
		  {
			return "delete-cascade";
		  }
	  }

	}

}