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
namespace org.camunda.bpm.qa.upgrade.util
{

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ExpectedActivityInstance
	{

	  /// <summary>
	  /// This is a list in some migration scenarios where the
	  /// activity id is not clear.
	  /// </summary>
	  protected internal IList<string> activityIds = new List<string>();
	  protected internal IList<ExpectedActivityInstance> childActivityInstances = new List<ExpectedActivityInstance>();
	  protected internal IList<ExpectedTransitionInstance> childTransitionInstances = new List<ExpectedTransitionInstance>();

	  public virtual IList<string> getActivityIds()
	  {
		return activityIds;
	  }
	  public virtual void setActivityIds(IList<string> activityIds)
	  {
		this.activityIds = activityIds;
	  }
	  public virtual void setActivityIds(string[] activityIds)
	  {
		this.activityIds = Arrays.asList(activityIds);
	  }
	  public virtual string ActivityId
	  {
		  set
		  {
			this.activityIds = Arrays.asList(value);
		  }
	  }
	  public virtual IList<ExpectedActivityInstance> ChildActivityInstances
	  {
		  get
		  {
			return childActivityInstances;
		  }
		  set
		  {
			this.childActivityInstances = value;
		  }
	  }
	  public virtual IList<ExpectedTransitionInstance> ChildTransitionInstances
	  {
		  get
		  {
			return childTransitionInstances;
		  }
		  set
		  {
			this.childTransitionInstances = value;
		  }
	  }

	  public override string ToString()
	  {
		StringWriter writer = new StringWriter();
		writeTree(writer, "", true);
		return writer.ToString();
	  }

	  protected internal virtual void writeTree(StringWriter writer, string prefix, bool isTail)
	  {
		writer.append(prefix);
		if (isTail)
		{
		  writer.append("└── ");
		}
		else
		{
		  writer.append("├── ");
		}

		writer.append(getActivityIds() + "\n");

		for (int i = 0; i < childTransitionInstances.Count; i++)
		{
		  ExpectedTransitionInstance transitionInstance = childTransitionInstances[i];
		  bool transitionIsTail = (i == (childTransitionInstances.Count - 1)) && (childActivityInstances.Count == 0);
		  writeTransition(transitionInstance, writer, prefix + (isTail ? "    " : "│   "), transitionIsTail);
		}

		for (int i = 0; i < childActivityInstances.Count; i++)
		{
		  ExpectedActivityInstance child = childActivityInstances[i];
		  child.writeTree(writer, prefix + (isTail ? "    " : "│   "), (i == (childActivityInstances.Count - 1)));
		}
	  }

	  protected internal virtual void writeTransition(ExpectedTransitionInstance transition, StringWriter writer, string prefix, bool isTail)
	  {
		writer.append(prefix);
		if (isTail)
		{
		  writer.append("└── ");
		}
		else
		{
		  writer.append("├── ");
		}

		writer.append("transition to/from " + transition.ActivityId + "\n");
	  }
	}

}