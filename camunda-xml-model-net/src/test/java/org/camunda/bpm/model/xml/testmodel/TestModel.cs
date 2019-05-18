﻿/*
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
namespace org.camunda.bpm.model.xml.testmodel
{
	using org.camunda.bpm.model.xml.testmodel.instance;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public sealed class TestModel
	{

	  private static Model model;
	  private static ModelBuilder modelBuilder;

	  public static Model getTestModel()
	  {
		if (model == null)
		{
		  ModelBuilder modelBuilder = ModelBuilder;

		  Animals.registerType(modelBuilder);
		  Animal.registerType(modelBuilder);
		  AnimalReference.registerType(modelBuilder);
		  Bird.registerType(modelBuilder);
		  ChildRelationshipDefinition.registerType(modelBuilder);
		  Description.registerType(modelBuilder);
		  FlightPartnerRef.registerType(modelBuilder);
		  FlyingAnimal.registerType(modelBuilder);
		  Guardian.registerType(modelBuilder);
		  GuardEgg.registerType(modelBuilder);
		  Mother.registerType(modelBuilder);
		  SpouseRef.registerType(modelBuilder);
		  FriendRelationshipDefinition.registerType(modelBuilder);
		  RelationshipDefinition.registerType(modelBuilder);
		  RelationshipDefinitionRef.registerType(modelBuilder);
		  Egg.registerType(modelBuilder);
		  FlightInstructor.registerType(modelBuilder);

		  Wings.registerType(modelBuilder);

		  model = modelBuilder.build();
		}

		return model;
	  }

	  public static ModelBuilder ModelBuilder
	  {
		  get
		  {
			if (modelBuilder == null)
			{
			  modelBuilder = ModelBuilder.createInstance(TestModelConstants.MODEL_NAME);
			}
			return modelBuilder;
		  }
	  }

	}

}