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
namespace org.camunda.bpm.engine.impl.@event
{
	/// <summary>
	/// Defines the existing event types, on which the subscription can be done.
	/// 
	/// Since the the event type for message and signal are historically lower case
	/// the enum variant can't be used, so we have to reimplement an enum like class.
	/// That is done so we can restrict the event types to only the defined ones.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public sealed class EventType
	{

	  public static readonly EventType MESSAGE = new EventType("message");
	  public static readonly EventType SIGNAL = new EventType("signal");
	  public static readonly EventType COMPENSATE = new EventType("compensate");
	  public static readonly EventType CONDITONAL = new EventType("conditional");

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private readonly string name_Renamed;

	  private EventType(string name)
	  {
		this.name_Renamed = name;
	  }

	  public string name()
	  {
		return name_Renamed;
	  }
	}

}