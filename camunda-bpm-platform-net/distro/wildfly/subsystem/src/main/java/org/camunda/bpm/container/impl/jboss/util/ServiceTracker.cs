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
namespace org.camunda.bpm.container.impl.jboss.util
{

	using AbstractServiceListener = org.jboss.msc.service.AbstractServiceListener;
	using ServiceController = org.jboss.msc.service.ServiceController;
	using State = org.jboss.msc.service.ServiceController.State;
	using Transition = org.jboss.msc.service.ServiceController.Transition;
	using ServiceName = org.jboss.msc.service.ServiceName;

	/// <summary>
	/// <para>Service Listener that adds / removes services to / from a collection as they
	/// are added / removed to the service controller.</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	/// @param <S>
	///          the type of the service to track </param>
	public class ServiceTracker<S> : AbstractServiceListener<object>
	{

	  protected internal ICollection<S> serviceCollection;
	  protected internal ServiceName typeToTrack;

	  public ServiceTracker(ServiceName typeToTrack, ICollection<S> serviceCollection)
	  {
		this.serviceCollection = serviceCollection;
		this.typeToTrack = typeToTrack;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public void transition(org.jboss.msc.service.ServiceController controller, org.jboss.msc.service.ServiceController.Transition transition)
	  public virtual void transition(ServiceController controller, ServiceController.Transition transition)
	  {

		if (!typeToTrack.isParentOf(controller.Name))
		{
		  return;
		}

		if (transition.After.State.Equals(ServiceController.State.UP))
		{
		  serviceCollection.Add((S) controller.Value);
		}
		else
		{
		  serviceCollection.remove(controller.Value);
		}

	  }

	}


}