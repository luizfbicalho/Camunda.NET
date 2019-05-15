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
namespace org.camunda.bpm.engine.impl.tree
{
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class ExecutionTopDownWalker : ReferenceWalker<ExecutionEntity>
	{

	  public ExecutionTopDownWalker(ExecutionEntity initialElement) : base(initialElement)
	  {
	  }

	  public ExecutionTopDownWalker(IList<ExecutionEntity> initialElements) : base(initialElements)
	  {
	  }

	  protected internal override ICollection<ExecutionEntity> nextElements()
	  {
		IList<ExecutionEntity> executions = CurrentElement.Executions;
		if (executions == null)
		{
		  executions = new List<ExecutionEntity>();
		}
		return (System.Collections.IList) executions;
	  }
	}

}