using System;

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
namespace org.camunda.bpm.integrationtest.functional.cdi.beans
{


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Named @ApplicationScoped public class TimerStartBean
	public class TimerStartBean
	{

	  public virtual string Cycle
	  {
		  get
		  {
			DateTime calendar = new DateTime();
			calendar = new DateTime(DateTime.Now);
			calendar.AddSeconds(10);
			DateTime startTime = calendar;
			return iso8601(startTime);
		  }
	  }

	  private string iso8601(DateTime date)
	  {
		DateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'");
		string dateStr = dateFormat.format(date);
		Console.WriteLine("Date: " + dateStr);
		return dateStr;
	  }
	}

}