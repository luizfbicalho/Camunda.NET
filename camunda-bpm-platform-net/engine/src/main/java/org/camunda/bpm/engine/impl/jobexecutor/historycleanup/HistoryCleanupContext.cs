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
namespace org.camunda.bpm.engine.impl.jobexecutor.historycleanup
{
	/// <summary>
	/// @author Svetlana Dorokhova.
	/// </summary>
	public class HistoryCleanupContext
	{

	  private bool immediatelyDue;
	  private int minuteFrom;
	  private int minuteTo;

	  public HistoryCleanupContext(bool immediatelyDue, int minuteFrom, int minuteTo)
	  {
		this.immediatelyDue = immediatelyDue;
		this.minuteFrom = minuteFrom;
		this.minuteTo = minuteTo;
	  }

	  public HistoryCleanupContext(int minuteFrom, int minuteTo)
	  {
		this.minuteFrom = minuteFrom;
		this.minuteTo = minuteTo;
	  }

	  public virtual bool ImmediatelyDue
	  {
		  get
		  {
			return immediatelyDue;
		  }
		  set
		  {
			this.immediatelyDue = value;
		  }
	  }


	  public virtual int MinuteFrom
	  {
		  get
		  {
			return minuteFrom;
		  }
		  set
		  {
			this.minuteFrom = value;
		  }
	  }


	  public virtual int MinuteTo
	  {
		  get
		  {
			return minuteTo;
		  }
		  set
		  {
			this.minuteTo = value;
		  }
	  }

	}

}