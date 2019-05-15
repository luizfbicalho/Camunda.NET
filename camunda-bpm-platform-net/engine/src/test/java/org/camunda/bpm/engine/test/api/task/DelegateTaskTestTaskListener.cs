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
namespace org.camunda.bpm.engine.test.api.task
{

	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;


	public class DelegateTaskTestTaskListener : TaskListener
	{

	  public const string VARNAME_CANDIDATE_USERS = "candidateUsers";
	  public const string VARNAME_CANDIDATE_GROUPS = "candidateGroups";

	  public virtual void notify(DelegateTask delegateTask)
	  {
		ISet<IdentityLink> candidates = delegateTask.Candidates;
		ISet<string> candidateUsers = new HashSet<string>();
		ISet<string> candidateGroups = new HashSet<string>();
		foreach (IdentityLink candidate in candidates)
		{
		  if (!string.ReferenceEquals(candidate.UserId, null))
		  {
			candidateUsers.Add(candidate.UserId);
		  }
		  else if (!string.ReferenceEquals(candidate.GroupId, null))
		  {
			candidateGroups.Add(candidate.GroupId);
		  }
		}
		delegateTask.setVariable(VARNAME_CANDIDATE_USERS, candidateUsers);
		delegateTask.setVariable(VARNAME_CANDIDATE_GROUPS, candidateGroups);
	  }

	}

}